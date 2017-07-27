using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadingText : MonoBehaviour
{

    void OnEnable()
    {
        GetComponent<Text>().CrossFadeAlpha(1, 0.3f, false);
    }

    void OnDisable()
    {
        GetComponent<Text>().CrossFadeAlpha(0, 0.3f, false);
    }

}