using UnityEngine;
using System.Collections;

public class FakeItemBox : LaunchableItem {

    void OnTriggerEnter(Collider coll)
    {
        CarCollCtl = coll.gameObject.GetComponent<CarCollisionController>();
        if (!CarCollCtl)
            return;
        CarCollCtl.HitItem();
        Hide("Box");
        Destroy(gameObject, duration);
    }

    void OnDestroy()
    {
        if (CarCollCtl)
        CarCollCtl.EnableCar(true);

        if (itemMgr)
            itemMgr.OnDefaultLaunch -= LaunchBackward;
    }

    void FixedUpdate()
    {
        FixedUpdateLaunchable();
    }
}
