using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class NavigableMenu : MonoBehaviour {

    private enum ControllerState
    {
        Mouse = 1,
        Keyboard = 2
    }
    private ControllerState crtState = ControllerState.Mouse;
    [SerializeField]
    GraphicRaycaster graphicRaycaster;

    private List<Button> buttons;
    private int selector;

    void Start ()
    {
        buttons = new List<Button>();
        Button[] tmpButtons = gameObject.GetComponentsInChildren<Button>();
        int index = 0;

        foreach (Button b in tmpButtons)
            if (b != null)
                if (b.GetComponent<EventTrigger>() != null)
                {
                    buttons.Add(b);
                    EventTrigger trigger = b.GetComponent<EventTrigger>();

                    EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry();
                    pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
                    pointerEnterEntry.callback.AddListener((data) => { SetSelector(index); });
                    trigger.triggers.Add(pointerEnterEntry);

                    EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry();
                    pointerExitEntry.eventID = EventTriggerType.PointerExit;
                    pointerExitEntry.callback.AddListener((data) => { SetSelector(0); });
                    trigger.triggers.Add(pointerExitEntry);

                    index++;
                }
        graphicRaycaster = GameObject.Find("Canvas").GetComponent<GraphicRaycaster>();

    }

    public void SetControllerState()
    {
        Event e = Event.current;
        if (e != null)
            if (crtState == ControllerState.Keyboard && e.isMouse || Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                crtState = ControllerState.Mouse;
                Cursor.visible = true;
                if (graphicRaycaster)
                    graphicRaycaster.enabled = true;
                EventSystem.current.SetSelectedGameObject(null);
            }
            else if (crtState == ControllerState.Mouse && e.isKey)
            {
                crtState = ControllerState.Keyboard;
                Cursor.visible = false;
                if (graphicRaycaster)
                    graphicRaycaster.enabled = false;
                buttons[selector].Select();
            }
    }

    public void SetSelector(int value)
    {
        selector = value;
    }

    void OnGUI()
    {
        SetControllerState();
    }
}
