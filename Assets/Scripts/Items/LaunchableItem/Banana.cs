using UnityEngine;
using System.Collections;

public class Banana : LaunchableItem {

    GameObject target;
    public GameObject Target { set { target = value; } }

    [SerializeField] float rotateSpeed;


    void FixedUpdate ()
    {
        FixedUpdateLaunchable();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (isLaunch)
        {
            RaycastHit groundHit;

            if (Physics.Raycast(transform.position, -transform.up, out groundHit))
            {
                if (groundHit.distance > 1f)
                    transform.position -= new Vector3(0, 9.8f, 0) * Time.fixedDeltaTime;
                else
                {
                    Vector3 position = transform.position;
                    position.y = groundHit.point.y + 1f;
                    transform.position = position;
                }
            }
        }*/

        if (target)
            target.transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }

    public override void LaunchForward()
    {
        if (owner)
        {
            transform.position = owner.gameObject.transform.position - new Vector3(0, -1f, launchDistance);
            transform.localEulerAngles = new Vector3(0, 0, 45f);
        }

        isLaunch = true;
    }

    void OnTriggerEnter(Collider coll)
    {
        CarCollCtl = coll.gameObject.GetComponent<CarCollisionController>();
        if (!CarCollCtl)
            return;
        CarCollCtl.HitItem();
        target = coll.gameObject.transform.root.gameObject;
        Hide("Capsule");
        Destroy(gameObject, duration);
    }

    void OnDestroy()
    {
        if (CarCollCtl)
            CarCollCtl.EnableCar(true);

        if (itemMgr)
            itemMgr.OnDefaultLaunch -= LaunchBackward;
    }
}
