using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{

    public Color color;
    Color defaultColor;
    Outline outline;

    void Start()
    {
        outline = GetComponentInChildren<Outline>();
        defaultColor = outline.effectColor;
    }

    public void OnSelect(BaseEventData eventData)
    {
        ApplyEffect();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        RemoveEffect();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ApplyEffect();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        RemoveEffect();
    }

    void ApplyEffect ()
    {
        if (outline)
            outline.effectColor = color;
        transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
    }

    void RemoveEffect ()
    {
        if (outline)
            outline.effectColor = defaultColor;
        transform.localScale = new Vector3(1, 1, 1);
    }
}
