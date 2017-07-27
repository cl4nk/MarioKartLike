using UnityEngine;
using System.Collections;
using Managers;

public class CameraController : MonoBehaviour {

    [Range(0, 180)]
    public float angle = 20;
    public float distance = 10;
    [Range(0, 1)]
    public float movementSmoothness = .4f;
    [Range(0, 1)]
    public float rotationSmoothness = .125f;
    [Range(0, 1)]
    public float tiltSmoothness = .05f;

    [Range(0, 180)]
    public float lowSpeedFOV = 60;
    [Range(0, 180)]
    public float highSpeedFOV = 100;
    [Range(0, 1)]
    public float cameraFOVSmoothness = 1 / 4f;

    public float turnAroundSpeed = 5;

    private float defaultMaxSpeed;

    [SerializeField]
    private GameObject player;

    private GameObject axisY;
    private GameObject cameraObject;

    private bool isTurnAround = false;
    
    void Awake ()
    {
        defaultMaxSpeed = player.GetComponent<CarController>().MaxSpeed;
    }

	void Start () {
        

        axisY = transform.Find("AxisY").gameObject;
        cameraObject = axisY.transform.Find("Camera").gameObject;

        transform.position = player.transform.position;
        transform.rotation = player.transform.rotation;
        cameraObject.transform.localPosition = new Vector3(0, 0, -distance);

        axisY.transform.localEulerAngles = new Vector3(angle, 0, 0);

        TrackManager.Instance.OnTrackFinish += TurnAround;
    }
	
	void FixedUpdate () {
        transform.position = Vector3.Lerp(
            transform.position, 
            player.transform.position, 
            movementSmoothness);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            player.transform.rotation,
            rotationSmoothness);

        if (!isTurnAround)
        {
            cameraObject.transform.localPosition = new Vector3(0, 0, -distance);
            cameraObject.transform.localRotation = Quaternion.Lerp(
                cameraObject.transform.localRotation,
                Quaternion.Euler(0, 0, axisY.transform.rotation.eulerAngles.y - player.transform.rotation.eulerAngles.y),
                tiltSmoothness);

            CarController carController = player.GetComponent<CarController>();
            cameraObject.GetComponent<Camera>().fieldOfView = Mathf.Lerp(
                cameraObject.GetComponent<Camera>().fieldOfView,
                Mathf.Lerp(lowSpeedFOV, highSpeedFOV, carController.Speed / Mathf.Max(carController.MaxSpeed, defaultMaxSpeed)),
                cameraFOVSmoothness);
        }
        else
        {
            axisY.transform.rotation = Quaternion.Lerp(
                axisY.transform.rotation,
                Quaternion.Euler(angle, axisY.transform.rotation.eulerAngles.y + turnAroundSpeed, 0),
                rotationSmoothness);

            cameraObject.transform.localPosition = new Vector3(0, 0, -distance);
        }
    }

    public void TurnAround()
    {
        isTurnAround = true;
    }
}
