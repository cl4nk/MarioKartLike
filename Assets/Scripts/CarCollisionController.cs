using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Items;
using Managers;

public class CarCollisionController : MonoBehaviour {

    [SerializeField] private float power = 3f;
    private bool invincible = false;
    public bool Invincible { get { return invincible; } set { invincible = value; } }

    private CarUserControl playerControl = null;
    private AICar enemyControl = null;
    private ItemManager itemMgr = null;

    void Start()
    {
        if (gameObject.tag == "Player")
        {
            playerControl = gameObject.GetComponentInParent<CarUserControl>();
            itemMgr = gameObject.GetComponentInParent<ItemManager>();
        }
        else
            enemyControl = gameObject.GetComponentInParent<AICar>();
    }

    public void EnableCar(bool state)
    {
        if (playerControl)
            playerControl.enabled = state;
        if (enemyControl)
            enemyControl.enabled = state;
        if (itemMgr)
            itemMgr.enabled = state;
    }

    public void StopCar()
    {
        gameObject.GetComponentInParent<CarController>().Speed = 0;
    }

    public void HitItem()
    {
        if (!invincible)
        {
            EnableCar(false);
            AudioSource[] sources = gameObject.GetComponents<AudioSource>();
            if (sources.Length == 2)
                AudioManager.Instance.PlayHit(gameObject.GetComponents<AudioSource>()[1]);
            StopCar();
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "LaunchableItem" && invincible)
        {
            Destroy(coll.gameObject);
        }
        else if (coll.gameObject.tag == "EnemyCollider" && invincible)
        {
            AICar ai = coll.gameObject.GetComponentInParent<AICar>();

            if (ai.isActiveAndEnabled)
            {
                ai.enabled = false;
                coll.gameObject.transform.parent.Translate(Vector3.up * power);
                StartCoroutine(ReactivateVictim(ai));
            }
        }
    }

    IEnumerator ReactivateVictim(AICar ai)
    {
        yield return new WaitForSeconds(5f);
        ai.enabled = true;
    }
}
