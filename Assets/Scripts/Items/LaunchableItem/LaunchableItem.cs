using UnityEngine;

namespace Items.LaunchableItem
{
    public class LaunchableItem : Item {

        [SerializeField] protected float launchDistance;
        [SerializeField] protected float dragDistance = 5f;
        protected bool isLaunch = false;

        protected CarCollisionController CarCollCtl;

        public Color particleColor;

        // Use this for initialization
        void Start () {
	
        }

        // Update is called once per frame
        public void FixedUpdateLaunchable()
        {
            if (!isLaunch && owner)
            {
                transform.position = (owner.transform.position - (owner.transform.forward * 5));
                transform.rotation = owner.transform.rotation;
            }
        }

        public override void SetOwner(GameObject newOwner)
        {
            owner = newOwner;
            itemMgr = owner.GetComponent<ItemManager>();
            if (itemMgr)
                AddDefaultLaunch();
        }

        public virtual void LaunchForward()
        {
            if (owner)
                transform.position = owner.gameObject.transform.position - new Vector3(0, -1f, launchDistance);

            isLaunch = true;
        }

        public virtual void LaunchBackward()
        {
            isLaunch = true;
        }

        public virtual void AddDefaultLaunch()
        {
            itemMgr.OnDefaultLaunch += LaunchBackward;
        }

        public void ShowParticles ()
        {
            GameObject particles = Resources.Load<GameObject>("Prefabs/ExplosionParticles");
            particles = Instantiate(particles, transform.position, transform.rotation) as GameObject;
            particles.GetComponent<ParticleSystemRenderer>().material.color = particleColor;
        }
        public void Hide(string ColliderType)
        {
        

            gameObject.GetComponent<Renderer>().enabled = false;

            switch (ColliderType)
            {
                case "Capsule":
                {   gameObject.GetComponent<CapsuleCollider>().enabled = false;     break;  }
                case "Box":
                {   gameObject.GetComponent<BoxCollider>().enabled = false;         break; }
                case "Sphere":
                {   gameObject.GetComponent<SphereCollider>().enabled = false;      break; }
                default:
                { break; }
            }
        }

        public void Show(string ColliderType)
        {
            gameObject.GetComponent<Renderer>().enabled = true;
            switch (ColliderType)
            {
                case "Capsule":
                { gameObject.GetComponent<CapsuleCollider>().enabled = true;    break; }
                case "Box":
                { gameObject.GetComponent<BoxCollider>().enabled = true;        break; }
                case "Sphere":
                { gameObject.GetComponent<SphereCollider>().enabled = true;     break; }
                default:
                { break; }
            }
        }
    }
}
