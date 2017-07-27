using UnityEngine;
using System.Collections;

public class CarController : MonoBehaviour, ICarController
{
    [SerializeField] private float maxSpeed = 80f; // m.s-1 
    [SerializeField] private float stopThreshold = 0.2f; // m.s-1
    [SerializeField] private float maxSteerAngle = 25f; // degrees
    [SerializeField] [Range(0f, 1f)] private float driftFactor = 0.15f;
    [SerializeField] private Transform frontWheelsCenter;
    [SerializeField] private Transform centerOfMass;

    private float speed = 0f; // m.s-1
    private float savedMaxSpeed = 0f; // Storage field in order to cap MaxSpeed temporarily
    private Vector3 velocity = Vector3.zero;
    private Vector3 rotationCenter = Vector3.zero;

    [SerializeField] private Transform mainAnchor; // The transform that will be used to determine the car's distance from the ground
    [SerializeField] private Transform[] satelliteAnchors = new Transform[3]; // These are used to find the ground plane our car is standing above
    [SerializeField] private float raycastUpDistance = 1f;
    [SerializeField] private float raycastDownDistance = 3f;

    private Vector3 satelliteAnchorsPlane;                            // Normal to the plane formed by the three satellite anchors
    private float raycastTotalDistance;
    private bool isFalling = false;

    [SerializeField] private float intoJumpThreshold = 5f;
    [SerializeField] private float outJumpThreshold = 0.1f;

    private float previousAltitude = 0f;

    [SerializeField] private GameObject[] wheelMeshes = new GameObject[6];

    private float previousSteering = 0f;

    private Rigidbody _rigidbody = null;

    private bool isKinematic = false;

    private Quaternion yawRotation = Quaternion.identity;             // Describes how the car should rotate around its up vector
    private Quaternion groundRotation = Quaternion.identity;          // Describes how the car should rotate to face the ground
    private Vector3 toGroundTranslation = Vector3.zero;               // Represents the car translation needed so that it sticks to the ground

    public float MaxSpeed { get { return maxSpeed; } }
    public Vector3 Velocity { get { return velocity; } set { velocity = value; } }
    public float Speed { get { return speed; } set { speed = value; } }
    public bool IsKinematic { get { return isKinematic; } set { isKinematic = value; } }
    
    
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        Vector3 anchorsCentroid = Vector3.zero;
        for (int i = 0; i < 3; ++i)
        {
            anchorsCentroid += .33f * satelliteAnchors[i].localPosition;
        }
        mainAnchor.localPosition = anchorsCentroid;

        raycastTotalDistance = raycastDownDistance + raycastUpDistance;

        ComputeSatelliteAnchorsPlane();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            if (speed > 30f)
                speed = 30f * Mathf.Sign(speed);
        }
    }

    public void Move(float steering, float acceleration, float brake, float boost)
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.angularDrag = 0f;
        _rigidbody.drag = 0f;

        yawRotation = Quaternion.identity;
        groundRotation = Quaternion.identity;
        toGroundTranslation = Vector3.zero;

        if (!isKinematic)
        {
            if (!isFalling)
            {
                ComputeVelocity(acceleration, Mathf.Abs(brake), boost);
                if (maxSpeed != 0f)
                {
                    ComputeYawRotation(steering);
                }
            }
            else
            {
                ApplyGravity();
            }

            CheckForFutureWallCollision();

            ApplyYawRotation(); 
            ApplyVelocity();

            //Debug.DrawRay(satelliteAnchors[0].position, velocity, Color.white, 0f, true);
            //Debug.DrawRay(transform.position, yawRotation * transform.forward, Color.yellow, 0f, true);
            //Debug.DrawRay(centerOfMass.position, transform.up, Color.green, 0f, true);

            if (!isFalling)
            {
                ComputeToGroundTranslation();
                ComputeGroundRotation();
            }

            ApplyGroundTranslation();
            ApplyGroundRotation();

            RotateWheels(steering);
            RecordAltitude();
        }
    }

    private void RotateWheels(float steering)
    {
        Quaternion deltaRotation = Quaternion.Euler(speed * 5f, 0f, 0f);
        foreach(GameObject wheel in wheelMeshes)
        {
            wheel.transform.rotation *= deltaRotation;
        }

        if (previousSteering != steering)
        {
            for (int i = 0; i < 4; ++i)
            {
                Quaternion cancelRotation = Quaternion.AngleAxis(-previousSteering * maxSteerAngle, Vector3.up);
                Quaternion steerRotation = Quaternion.AngleAxis(steering * maxSteerAngle, Vector3.up);
                wheelMeshes[i].transform.parent.rotation *= cancelRotation;
                wheelMeshes[i].transform.parent.rotation *= steerRotation;
            }
        }

        previousSteering = steering;
    }

    private void ApplyGravity()
    {
        velocity += Vector3.down * 9.81f * Time.fixedDeltaTime * 2f;
    }

    private void ApplyYawRotation()
    {
        transform.rotation *= yawRotation;
    }

    private void ApplyVelocity()
    {
        transform.position += velocity * Time.fixedDeltaTime;
    }

    private void ApplyGroundTranslation()
    {
        transform.position += toGroundTranslation;
    }

    private void ApplyGroundRotation()
    {
        float angle; Vector3 axis;
        groundRotation.ToAngleAxis(out angle, out axis);
        transform.RotateAround(mainAnchor.position, axis, angle);
    }

    private void RecordAltitude()
    {
        float recordedAltitude = Mathf.Infinity;

        Ray altitudeRay = new Ray(mainAnchor.position + transform.up * raycastUpDistance, -transform.up);
        RaycastHit altitudeHitInfo;
        if (Physics.Raycast(altitudeRay, out altitudeHitInfo))
        {
            recordedAltitude = altitudeHitInfo.distance - raycastUpDistance;
        }

        if (recordedAltitude < 0.001f)
            recordedAltitude = 0f;

        if (!isFalling)
        {
            if (recordedAltitude > previousAltitude + intoJumpThreshold)
                isFalling = true;
        }
        else
        {
            if (recordedAltitude < outJumpThreshold)
                isFalling = false;
        }

        previousAltitude = recordedAltitude;
    }

    private void ComputeVelocity(float acceleration, float brake, float boost)
    {
        speed = Mathf.Lerp(speed, maxSpeed, Time.fixedDeltaTime * .2f * acceleration);
        speed = Mathf.Lerp(speed, -maxSpeed / 2, Time.fixedDeltaTime * .3f * brake);
        speed += boost * Time.fixedDeltaTime;

        if (boost == 0f)
        {
            if (speed > maxSpeed)
            {
                speed = Mathf.Lerp(speed, maxSpeed, 0.1f);
            }
        }

        if (acceleration == 0f && brake == 0f)
        {
            speed = Mathf.Lerp(speed, 0f, Time.fixedDeltaTime * 1f); // Apply ground friction
            if (Mathf.Abs(speed) < stopThreshold)
                speed = 0f;
        }

        rotationCenter = ULerp(frontWheelsCenter.position, centerOfMass.position, (Mathf.Abs(speed) / maxSpeed) + driftFactor);

        velocity = transform.forward * speed;
    }

    private void ComputeYawRotation(float steering)
    {
        Vector3 wheelsDirection = Quaternion.Euler(transform.up * (steering * maxSteerAngle)) * transform.forward;

        float wheelsSpeed = Vector3.Dot(velocity, wheelsDirection);
        Vector3 wheelsNormal = Vector3.Cross(transform.up, wheelsDirection).normalized;

        float rotationRadius = Vector3.Dot(rotationCenter - frontWheelsCenter.position, wheelsNormal);
        
        float angularSpeed = 0f;
        if (Mathf.Abs(rotationRadius) <= 0.01f)
            rotationRadius = 0f;
        else
            angularSpeed = wheelsSpeed / rotationRadius;

        yawRotation = Quaternion.AngleAxis(angularSpeed * Time.fixedDeltaTime, Vector3.up);
        /*
        if (gameObject.tag == "Player")
        {
            Debug.Log(angularSpeed);
            Debug.Log(wheelsSpeed);
            Debug.Log(rotationRadius);
            Debug.Log("");
        }
        */
    }

    private void ComputeToGroundTranslation()
    {
        Ray mainRay = new Ray(mainAnchor.position + transform.up * raycastUpDistance, -transform.up); // Offset the origin of the ray upwards by upDistance, so that it can be casted down
        RaycastHit mainHitInfo;
        if (Physics.Raycast(mainRay, out mainHitInfo, raycastTotalDistance))
        {
            if (mainHitInfo.collider.CompareTag("Wall"))
            {
                toGroundTranslation = (mainHitInfo.distance - raycastUpDistance) * mainRay.direction; // Remove the offset to position the car at the right place
            }
        }

        Debug.DrawRay(mainRay.origin, mainRay.direction * raycastTotalDistance, Color.cyan, 0f, true);
    }

    private void ComputeGroundRotation()
    {
        bool everyRaycastHit = true;
        Vector3[] anchorsGroundProjections = new Vector3[3];
        for (int i = 0; i < 3; ++i)
        {
            anchorsGroundProjections[i] = satelliteAnchors[i].position;

            Ray satelliteRay = new Ray(satelliteAnchors[i].position + transform.up * raycastUpDistance, -transform.up);
            RaycastHit satelliteHitInfo;
            if (Physics.Raycast(satelliteRay, out satelliteHitInfo, raycastDownDistance))
            {
                if (satelliteHitInfo.collider.CompareTag("Wall"))
                    anchorsGroundProjections[i] = satelliteHitInfo.point;
            }
            else
            {
                everyRaycastHit &= false;
            }

            //Debug.DrawRay(satelliteRay.origin, satelliteRay.direction * raycastTotalDistance, Color.cyan, 0f, true);
            //Debug.DrawLine(mainAnchor.position, anchorsGroundProjections[i], Color.blue, 0f, true);
        }

        if (everyRaycastHit)
        {
            Vector3 groundPlaneNormal = Vector3.Cross(anchorsGroundProjections[2] - anchorsGroundProjections[0],
                                                      anchorsGroundProjections[1] - anchorsGroundProjections[0]).normalized;

            groundRotation = Quaternion.FromToRotation(transform.rotation * satelliteAnchorsPlane, groundPlaneNormal);

            //Debug.DrawRay(mainAnchor.position, transform.rotation * satelliteAnchorsPlane, Color.magenta, 0f, true);
            //Debug.DrawRay(mainAnchor.position + transform.forward, groundPlaneNormal, Color.red, 0f, true);
        }
        else
        {
            isFalling = true;
        }

    }

    private void CheckForFutureWallCollision()
    {
        int layerMask = 1 << LayerMask.NameToLayer("ItemCollider");
        layerMask &= 1 << LayerMask.NameToLayer("RacerCollider");
        layerMask = ~layerMask;

        Ray forwardRay = new Ray(transform.position, transform.forward);
        RaycastHit forwardHitInfo;
        if (Physics.Raycast(forwardRay, out forwardHitInfo, 3.5f + speed * Time.fixedDeltaTime, layerMask))
        {
            if (Mathf.Abs(Vector3.Dot(transform.up, forwardHitInfo.normal) - transform.up.magnitude) > 0.5f)
            {
            if (speed > 30f)
                speed = 30f * Mathf.Sign(speed);
            }
        }
    }

    private void ComputeSatelliteAnchorsPlane()
    {
        satelliteAnchorsPlane = Vector3.Cross(satelliteAnchors[2].localPosition - satelliteAnchors[0].localPosition,
                                                satelliteAnchors[1].localPosition - satelliteAnchors[0].localPosition).normalized;
    }

    public void SetTopspeed(float speed)
    {
        
        if (savedMaxSpeed == 0f)
        {
            savedMaxSpeed = maxSpeed;
            maxSpeed = speed;
            this.speed = maxSpeed;
        }
    }

    public void UnsetTopspeed()
    {
        
        if (savedMaxSpeed != 0f)
        {
            maxSpeed = savedMaxSpeed;
            savedMaxSpeed = 0f;
        }
    }

    private Vector3 ULerp(Vector3 from, Vector3 to, float t)
    {
        return from * (1 - t) + to * t;
    }
}
