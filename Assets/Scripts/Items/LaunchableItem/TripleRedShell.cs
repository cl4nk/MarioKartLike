using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TripleRedShell : LaunchableItem {

    List<RedShell> children;
    public List<RedShell> Children { get { return children; } }
    [SerializeField] float rotateAngle = 20;

	// Use this for initialization
	void Start () {
        children = new List<RedShell>();
        foreach (RedShell shell in GetComponentsInChildren<RedShell>())
            children.Add(shell);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!isLaunch)
        {
            transform.position = owner.transform.position;
            transform.rotation = owner.transform.rotation;
        }

        foreach (RedShell child in children)
            child.transform.RotateAround(transform.position, Vector3.up, rotateAngle * Time.fixedDeltaTime);
	}

    public override void LaunchForward()
    {
        RedShell shell = children[children.Count - 1];
        
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
        RedShell shell = children[children.Count - 1];

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
