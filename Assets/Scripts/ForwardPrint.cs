using UnityEngine;
using System.Collections;

public class ForwardPrint : MonoBehaviour {

    public Color color;
	
    void OnDrawGizmos ()
    {
        Gizmos.color = color;
        Gizmos.DrawLine(transform.position, transform.forward * 5 + transform.position );
        Gizmos.DrawSphere(transform.position + transform.forward * 5, 1f);
    }
}
