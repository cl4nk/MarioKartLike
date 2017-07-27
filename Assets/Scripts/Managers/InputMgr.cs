using UnityEngine;
using System.Collections;

public class InputMgr : MonoBehaviour
{
    static InputMgr instance = null;
    static public InputMgr Instance
    {
        get
        {
            if (instance == null)
			{
				InputMgr target = GameObject.FindObjectOfType<InputMgr>();
				if (target != null)
				{
					instance = target;
				}
			}
			return instance;
        }
    }

    public delegate void InputKey();
    public event InputKey spaceIsDown;
    public event InputKey spaceIsUp;
    public event InputKey ObjectBackIsDown;
    public event InputKey ObjectBackIsUp;
    public event InputKey escapeIsDown;
    
    public delegate void InputAxis(float axis);
    public event InputAxis steering;
    public event InputAxis accel;
    public event InputAxis brake;
    
    private bool isBroadcasting = true;

    public void SetBroadcasting(bool active)
    {
        isBroadcasting = active;
    }

    void CheckInput()
    {   
        if (steering != null)
            steering(Input.GetAxis("Horizontal"));
        if (brake != null)
            brake(Input.GetAxis("Backward"));
        if (accel != null)
            accel(Input.GetAxis("Forward"));
        //if (Input.GetButtonDown("UseObject") && spaceIsDown != null)
        //    spaceIsDown();
        //if (Input.GetButtonUp("UseObject") && spaceIsUp != null)
        //    spaceIsUp();
        //if (Input.GetButtonUp("ObjectBack") && ObjectBackIsUp != null)
        //    ObjectBackIsUp();
        //if (Input.GetButtonDown("ObjectBack") && ObjectBackIsDown != null)
        //    ObjectBackIsDown();
        if (Input.GetKeyDown(KeyCode.Escape) && escapeIsDown != null)
            escapeIsDown();
    }

	void Update () 
    {
        if(isBroadcasting)
            CheckInput();
	}
}
