using UnityEngine;
using System.Collections;

public class ItemBox : MonoBehaviour {

    Item[] itemPrefabs;
    [SerializeField] float rotateSpeed;
    [SerializeField] float respawnTime;
    

	// Use this for initialization
	void Start () {
        itemPrefabs = GameManager.Instance.itemPrefabs;
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
	}

    private void Hide()
    {
        gameObject.GetComponent<BoxCollider>().enabled = false;
        gameObject.GetComponent<Renderer>().enabled = false;

        GetComponent<AudioSource>().Play();

        GameObject particles = Resources.Load<GameObject>("Prefabs/ItemBoxParticles");
        particles = Instantiate(particles, transform.position, transform.rotation) as GameObject;
    }

    private void Show()
    {
        gameObject.GetComponent<BoxCollider>().enabled = true;
        gameObject.GetComponent<Renderer>().enabled = true;
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "Player" || coll.gameObject.tag == "Enemy")
        {
            ItemManager itemMgr = coll.gameObject.GetComponentInParent<ItemManager>();

            if (itemMgr)
            {
                int value = Random.Range(0, itemPrefabs.Length);
                itemMgr.setItem(itemPrefabs[value]);
            }

            Hide();
            StartCoroutine(WaitForRespawn());
        }
    }

    IEnumerator WaitForRespawn()
    {
        yield return new WaitForSeconds(respawnTime);
        Show();
    }
}
