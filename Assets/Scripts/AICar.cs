using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AICar : MonoBehaviour
{
    private CarController controller = null;
    private CarInfo carInfo = null;

    private float steering = 0f;
    private float acceleration = 0f;
    private float brake = 0f;
    private float boost = 0f;

    public float Boost { get { return boost; } set { boost = value; } }

    public float angleAccelMin = 35f;
    private float carInFrontDistance = 15f;

    private Vector3 carInFrontHitPoint;
    private float carInFrontSpeed;

    private float carOnSideDistance = 1f;
    private float wallOnSideDistance = 1f;

    private Vector3 carOnLeftHitPoint;
    private Vector3 carOnRightHitPoint;
    private Vector3 wallOnLeftHitPoint;
    private Vector3 wallOnRightHitPoint;

    Vector3 pointToGo;

	// Use this for initialization
	void Start () {
        controller = GetComponent<CarController>();
        carInfo = GetComponent<CarInfo>();
    }
	

    void FixedUpdate ()
    {
        Vector3 checkPtVec = TrackManager.Instance.GetCheckPointVec(carInfo.NextPoint);
        Vector3 checkPtPos = TrackManager.Instance.GetCheckPointPos(carInfo.NextPoint);
        Vector3 start = checkPtPos + checkPtVec * carInfo.halfSegmentSize;
        Vector3 end = checkPtPos - checkPtVec * carInfo.halfSegmentSize;
        pointToGo = GetPointNearestBetween(start, end);
        Vector3 localNextPoint = transform.InverseTransformPoint(pointToGo);
        localNextPoint.y = 0;

        float angle = Mathf.Abs(Vector3.Angle(Vector3.forward, localNextPoint));

        bool slowDown = CarInFront() && (carInFrontSpeed < GetComponent<CarController>().Speed);
        acceleration = slowDown ? 0f : Mathf.Max((angleAccelMin - angle) / angleAccelMin, 0);
        brake = (slowDown && GetComponent<CarController>().Speed > 0) ? 1f : 0f;

        if (WallOnRight())
            steering = -1;
        else if (WallOnLeft())
            steering = 1;
        else if (CarOnLeft() && !WallOnRight())
            steering = 1;
        else if (CarOnRight() && !WallOnLeft())
            steering = -1;
        else
            steering = Mathf.Clamp((int)localNextPoint.x, -1, 1);
        if (controller.Speed == 0)
            acceleration = 1f;
        controller.Move(steering, acceleration, brake, boost);
    }

    

    void OnDrawGizmos()
    {
        if (carInfo == null)
            return;
        Vector3 nextPointVec = TrackManager.Instance.GetCheckPointPos(carInfo.LastPoint);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(nextPointVec, 2f);

        if (CarInFront())
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(carInFrontHitPoint, 1f);
            Gizmos.DrawLine(transform.position + new Vector3(0, 1, 0) - transform.right, carInFrontHitPoint);
            Gizmos.DrawLine(transform.position + new Vector3(0, 1, 0) + transform.right, carInFrontHitPoint);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position + new Vector3(0, 1, 0) - transform.right, transform.forward);
            Gizmos.DrawRay(transform.position + new Vector3(0, 1, 0) + transform.right, transform.forward);
        }

        if (CarOnLeft())
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(carOnLeftHitPoint, 1f);
            Gizmos.DrawLine(transform.position + new Vector3(0, 1, 0) - transform.right, carOnLeftHitPoint);
        }

        if (CarOnRight())
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(carOnRightHitPoint, 1f);
            Gizmos.DrawLine(transform.position + new Vector3(0, 1, 0) + transform.right, carOnRightHitPoint);
        }

        if (WallOnLeft())
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(wallOnLeftHitPoint, 1f);
            Gizmos.DrawLine(transform.position + new Vector3(0, 1, 0) - transform.right, wallOnLeftHitPoint);
        }

        if (WallOnRight())
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(wallOnRightHitPoint, 1f);
            Gizmos.DrawLine(transform.position + new Vector3(0, 1, 0) + transform.right, wallOnRightHitPoint);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(pointToGo, 1f);
    }

    bool CarInFront()
    {
        RaycastHit hitLeft;
        RaycastHit hitRight;

        bool hasHitLeft  = Physics.Raycast(transform.position + new Vector3(0, 1, 0) - transform.right, transform.forward, out hitLeft);
        bool hasHitRight = Physics.Raycast(transform.position + new Vector3(0, 1, 0) + transform.right, transform.forward, out hitRight);

        float hitLeftDistance = hitLeft.distance;
        float hitRightDistance = hitRight.distance;

        if (hasHitLeft && hasHitRight)
        {
            if (hitLeftDistance < hitRightDistance)
            {
                carInFrontHitPoint = hitLeft.point;
                bool result = hitLeft.collider.name == "AreaEffect" && hitLeft.distance < carInFrontDistance;
                if (result) carInFrontSpeed = hitLeft.collider.transform.parent.GetComponent<CarController>().Speed;
                return result;
            }
            else
            {
                carInFrontHitPoint = hitRight.point;
                bool result = hitRight.collider.name == "AreaEffect" && hitRight.distance < carInFrontDistance;
                if (result) carInFrontSpeed = hitRight.collider.transform.parent.GetComponent<CarController>().Speed;
                return result;
            }
        }
        else if (hasHitLeft && !hasHitRight)
        {
            carInFrontHitPoint = hitLeft.point;
            bool result = hitLeft.collider.name == "AreaEffect" && hitLeft.distance < carInFrontDistance;
            if (result) carInFrontSpeed = hitLeft.collider.transform.parent.GetComponent<CarController>().Speed;
            return result;
        }
        else if (!hasHitLeft && hasHitRight)
        {
            carInFrontHitPoint = hitRight.point;
            bool result = hitRight.collider.name == "AreaEffect" && hitRight.distance < carInFrontDistance;
            if (result) carInFrontSpeed = hitRight.collider.transform.parent.GetComponent<CarController>().Speed;
            return result;
        }

        return false;
    }

    bool CarBefore ()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + new Vector3(0, 1, 0) +transform.forward, transform.forward, out hit))
        {
            bool result = hit.collider.name == "AreaEffect";
            return result;
        }

        return false;
    }

    bool CarOnLeft()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + new Vector3(0, 1, 0) - transform.right, -transform.right, out hit))
        {
            carOnLeftHitPoint = hit.point;
            bool result = hit.collider.name == "AreaEffect" && hit.distance < carOnSideDistance;
            return result;
        }

        return false;
    }

    bool WallOnLeft()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + new Vector3(0, 1, 0) - transform.right, -transform.right, out hit))
        {
            wallOnLeftHitPoint = hit.point;
            bool result = hit.collider.tag == "Wall" && hit.distance < wallOnSideDistance;
            return result;
        }

        return false;
    }

    bool CarOnRight()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + new Vector3(0, 1, 0) + transform.right, transform.right, out hit))
        {
            carOnRightHitPoint = hit.point;
            bool result = hit.collider.name == "AreaEffect" && hit.distance < carOnSideDistance;
            return result;
        }

        return false;
    }

    bool WallOnRight()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + new Vector3(0, 1, 0) + transform.right, transform.right, out hit))
        {
            wallOnRightHitPoint = hit.point;
            bool result = hit.collider.tag == "Wall" && hit.distance < wallOnSideDistance;
            return result;
        }

        return false;
    }

    Vector3 GetPointNearestBetween(Vector3 start, Vector3 end)
    {
        float diff = (end - start).magnitude;
        if (diff < 2f)
            return start;
        float startDistance = transform.InverseTransformPoint(start).magnitude;
        float endDistance = transform.InverseTransformPoint(end).magnitude;
        if (startDistance < endDistance)
            return GetPointNearestBetween(start, (end - start) / 2 + start);
        else
            return GetPointNearestBetween(end, (start - end) / 2 + end);

        
    }
}
