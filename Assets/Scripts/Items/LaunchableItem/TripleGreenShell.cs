using System.Collections.Generic;
using UnityEngine;

namespace Items.LaunchableItem
{
    public class TripleGreenShell : LaunchableItem
    {

        List<GreenShell> children;
        public List<GreenShell> Children { get { return children; } }
        [SerializeField] float rotateAngle = 20;

        // Use this for initialization
        void Start()
        {
            children = new List<GreenShell>();
            foreach (GreenShell shell in GetComponentsInChildren<GreenShell>())
                children.Add(shell);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (!isLaunch)
            {
                transform.position = owner.transform.position;
                transform.rotation = owner.transform.rotation;
            }

            foreach (GreenShell child in children)
            {
                child.transform.RotateAround(transform.position, Vector3.up, rotateAngle * Time.fixedDeltaTime);
            }
        }

        public override void LaunchForward()
        {
            GreenShell shell = children[children.Count - 1];
        
            if (owner)
                shell.SetOwner(owner);

            shell.transform.parent = null;
            shell.LaunchForward();
            children.Remove(shell);

            if (children.Count == 0)
            {
                itemMgr.OnDefaultLaunch -= LaunchForward;
                owner.GetComponent<ItemManager>().destroyTriple();
            }
        }

        public override void LaunchBackward()
        {
            GreenShell shell = children[children.Count - 1];

            if (owner)
                shell.SetOwner(owner);

            shell.transform.parent = null;
            shell.LaunchBackward();
            children.Remove(shell);

            if (children.Count == 0)
            {
                itemMgr.OnDefaultLaunch -= LaunchForward;
                owner.GetComponent<ItemManager>().destroyTriple();
            }
        }

        public override void AddDefaultLaunch()
        {
            itemMgr.OnDefaultLaunch += LaunchForward;
        }
    }
}