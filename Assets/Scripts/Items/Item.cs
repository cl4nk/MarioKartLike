using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {

    protected GameObject owner = null;
    [SerializeField] protected string type;
    public string Type { get { return type; } }
    protected ItemManager itemMgr;

    [SerializeField] protected float duration;

    public virtual void SetOwner(GameObject newOwner)
    {
        owner = newOwner;
        itemMgr = owner.GetComponent<ItemManager>();
    }
}
