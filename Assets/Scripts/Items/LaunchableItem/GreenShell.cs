using UnityEngine;

namespace Items.LaunchableItem
{
    public class GreenShell : LaunchableItem {

        [SerializeField] private float speedMultiply;
        private float speed;

        private Vector3 shell_velocity;
        private int rebound = 0;
        [SerializeField] private int maxRebound;
        private Vector3 direction;

        public string Print = "";


        void FixedUpdate()
        {
            FixedUpdateLaunchable();
            if (isLaunch)
            {
                transform.position += transform.forward * Mathf.Max(speed, 1f) * Time.deltaTime;
                RaycastHit groundHit;

                if (Physics.Raycast(transform.position, -transform.up, out groundHit))
                {
                    if (groundHit.distance > 1f)
                    {
                        //transform.position -= new Vector3(0, 9.8f, 0) * Time.fixedDeltaTime;
                        transform.LookAt(transform.forward + transform.position - new Vector3(0, 9.8f, 0) * Time.fixedDeltaTime);
                    }
                    else if (groundHit.distance < .8f)
                    {
                        Vector3 position = transform.position;
                        position.y = groundHit.point.y + .95f;
                        transform.position = position;
                        Vector3 target = (transform.forward + transform.position);
                        target.y = groundHit.point.y + .95f;
                        //transform.LookAt((transform.forward + transform.position) + new Vector3(0, 9.8f, 0) * Time.fixedDeltaTime);
                    }
                
                }
            
            }
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
                    (owner.gameObject.transform.forward * launchDistance);
                speed = owner.GetComponent<CarController>().Speed * speedMultiply;
            
            }

            GetComponent<SphereCollider>().isTrigger = false;

            //transform.eulerAngles = Vector3.forward;

            isLaunch = true;

            if (itemMgr)
                itemMgr.OnDefaultLaunch -= LaunchForward;
        }

        public override void LaunchBackward()
        {
            transform.eulerAngles = Vector3.back;

            isLaunch = true;

            GetComponent<SphereCollider>().isTrigger = false;

            if (itemMgr)
                itemMgr.OnDefaultLaunch -= LaunchForward;
        }

        void OnTriggerEnter(Collider coll)
        {
            if (coll.gameObject.tag == "EnemyCollider")
            {
                TripleGreenShell parent = transform.root.gameObject.GetComponent<TripleGreenShell>();
                if (parent)
                    parent.Children.Remove(gameObject.GetComponent<GreenShell>());
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
        
            if (coll.collider.CompareTag("Wall") && isLaunch)
            {
                Debug.Log(transform.eulerAngles);
                foreach (ContactPoint contact in coll.contacts)
                {
                    Vector3 reflectDir = Vector3.Reflect(transform.forward, contact.normal);
                    transform.LookAt(reflectDir + transform.position);
                }
           
                rebound += 1;
                if (rebound >= maxRebound)
                {
                    ShowParticles();
                    Destroy(gameObject);
                }
                else
                    GetComponent<AudioSource>().Play();
            }

        
        }

        void OnDestroy()
        {
            TripleGreenShell parent = transform.root.gameObject.GetComponent<TripleGreenShell>();
            if (parent)
                parent.Children.Remove(gameObject.GetComponent<GreenShell>());

            if (CarCollCtl)
                CarCollCtl.EnableCar(true);
            if (itemMgr)
                itemMgr.OnDefaultLaunch -= LaunchForward;
        }

        void CheckWall(Vector3 reflectDir)
        {
            RaycastHit hit;

            float leftDistance = 100f;
            float rightDistance = 100f;

            if (Physics.Raycast(transform.position, -transform.right, out hit))
                leftDistance = hit.distance;
            if (Physics.Raycast(transform.position, transform.right, out hit))
                rightDistance = hit.distance;

            if (leftDistance > rightDistance)
            {
                print("right");
                transform.rotation = Quaternion.FromToRotation(transform.position, -reflectDir);
            }
            else
            {
                print("left");
                transform.rotation = Quaternion.FromToRotation(transform.position, reflectDir);
            }

        }
    }
}
