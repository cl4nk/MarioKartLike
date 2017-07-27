using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RedShell : LaunchableItem {

    private GameObject target;

    [SerializeField] private float speed;

    private int nextPoint;
    private float reachDistance = 2f;

    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        FixedUpdateLaunchable();
        if (isLaunch)
        {
            transform.position += transform.forward * speed * Time.fixedDeltaTime;

            ApplyGravity();

            if (target)
            {
                CheckPointToGo();
                CheckNextPoint();
                CheckIfTargetInRange();
            }
        }
    }

    void ApplyGravity()
    {
        RaycastHit groundHit;

        if (Physics.Raycast(transform.position, -transform.up, out groundHit))
        {
            if (groundHit.distance > 1f)
                transform.position -= new Vector3(0, 9.8f, 0) * Time.fixedDeltaTime;
            else if (groundHit.distance < .8f)
            {
                Vector3 position = transform.position;
                position.y = groundHit.point.y + .95f;
                transform.position = position;
            }
        }
    }

    void CheckPointToGo()
    {
        Vector3 pointToGo = TrackManager.Instance.GetCheckPointPos(nextPoint);
        pointToGo.y = transform.position.y;
        transform.LookAt(pointToGo);
    }

    void CheckNextPoint()
    {
        Vector3 inverseNextPointVec = transform.InverseTransformPoint(TrackManager.Instance.GetCheckPointPos(nextPoint));
        inverseNextPointVec.y = 0;

        if (inverseNextPointVec.magnitude <= reachDistance)
            nextPoint = TrackManager.Instance.GetNextCheckPointIndex(nextPoint);
    }

    void CheckIfTargetInRange()
    {
        Collider[] collArray = Physics.OverlapBox(transform.position, new Vector3(3, 3, 3));

        foreach (Collider coll in collArray)
            if (coll.gameObject.transform.root.gameObject == target)
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, 0.5f);
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "EnemyCollider")
        {
            TripleRedShell parent = transform.root.gameObject.GetComponent<TripleRedShell>();
            if (parent)
                parent.Children.Remove(gameObject.GetComponent<RedShell>());
            CarCollCtl = coll.gameObject.GetComponent<CarCollisionController>();
            CarCollCtl.HitItem();
            ShowParticles();
            Hide("Sphere");
            Destroy(gameObject, duration);
        }

        if (coll.gameObject.tag == "LaunchableItem")
        {
            ShowParticles();
            Destroy(coll.gameObject);
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.tag == "Player" || coll.gameObject.tag == "Enemy")
        {
            CarCollCtl = coll.gameObject.GetComponent<CarCollisionController>();
            if (!CarCollCtl)
                return;
            CarCollCtl.HitItem();

            Hide("Sphere");
            Destroy(gameObject, duration);
            return;
        }

        if (coll.gameObject.tag == "LaunchableItem")
        {
            Destroy(coll.gameObject);
            Destroy(gameObject);
        }

        if (coll.collider.CompareTag("Wall") && isLaunch)
        {
            ShowParticles();
            Destroy(gameObject);
         
        }
    }

    void OnDestroy()
    {
        TripleRedShell parent = transform.root.gameObject.GetComponent<TripleRedShell>();
        if (parent)
            parent.Children.Remove(gameObject.GetComponent<RedShell>());

        if (CarCollCtl)
            CarCollCtl.EnableCar(true);
        if (itemMgr)
            itemMgr.OnDefaultLaunch -= LaunchForward;
    }

    public override void AddDefaultLaunch()
    {
        itemMgr.OnDefaultLaunch += LaunchForward;
    }

    public override void LaunchForward()
    {
        if (owner)
        {
            transform.position =
                owner.gameObject.transform.position +
                new Vector3(0, 1f, 0) +
                (owner.gameObject.transform.forward * launchDistance); ;
            nextPoint = owner.GetComponent<CarInfo>().NextPoint;
        }
        target = TrackManager.Instance.GetCarBefore(owner).gameObject;

        isLaunch = true;
        GetComponent<SphereCollider>().isTrigger = false;
        GetComponent<AudioSource>().Play();

        if (itemMgr)
            itemMgr.OnDefaultLaunch -= LaunchForward;
        
    }

    public override void LaunchBackward()
    {
        transform.eulerAngles = Vector3.back;

        isLaunch = true;
        GetComponent<SphereCollider>().isTrigger = false;
        GetComponent<AudioSource>().Play();

        if (itemMgr)
            itemMgr.OnDefaultLaunch -= LaunchForward;
        
    }
}
