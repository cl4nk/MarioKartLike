using UnityEngine;
using System.Collections;
using Managers;


public class CarUserControl : MonoBehaviour
{
    private ICarController controller = null;

    private float steering = 0f;
    private float acceleration = 0f;
    private float brake = 0f;
    private float boost = 0f;
    private bool registered = false;

    public float Boost { get { return boost; } set { boost = value; } }

    public void RegisterInputFunctions()
    {
        if(registered == false)
        {
            InputMgr.Instance.spaceIsDown += OnSpaceDown;
            InputMgr.Instance.spaceIsUp += OnSpaceUp;
            InputMgr.Instance.ObjectBackIsDown += OnBackObjectDown;
            InputMgr.Instance.ObjectBackIsUp += OnBackObjectUp;
            InputMgr.Instance.steering += SetSteering;
            InputMgr.Instance.brake += SetBrake;
            InputMgr.Instance.accel += SetAcceleration;

            registered = true;
        }
    }

    public void UnregisterInputFunctions()
    {
        if(registered == true)
        {
            InputMgr.Instance.spaceIsDown -= OnSpaceDown;
            InputMgr.Instance.spaceIsUp -= OnSpaceUp;
            InputMgr.Instance.ObjectBackIsDown -= OnBackObjectDown;
            InputMgr.Instance.ObjectBackIsUp -= OnBackObjectUp;
            InputMgr.Instance.steering += SetSteering;
            InputMgr.Instance.brake += SetBrake;
            InputMgr.Instance.accel += SetAcceleration;

            registered = false;
        }
    }

    void Awake()
    {
        controller = GetComponent<ICarController>();

        RegisterInputFunctions();
    }

    void OnDestroy()
    {
        if (InputMgr.Instance != null)
            UnregisterInputFunctions();
    }

    void FixedUpdate()
    {
        if (registered == true)
        {
            controller.Move(steering, acceleration, brake, boost);
        }
        else
        {
            controller.Move(0f, 0f, 0f, 0f);
        }
    }

    void SetSteering(float axis)
    {
        steering = axis;
    }

    void SetAcceleration(float axis)
    {
        acceleration = axis;
    }

    void SetBrake(float axis)
    {
        brake = axis;
    }

    void OnSpaceDown()
    {
    }

    void OnSpaceUp()
    {
    }

    void OnBackObjectDown()
    {
    }

    void OnBackObjectUp()
    {
    }
}
