using UnityEngine;
using System.Collections;

public class Champignon : UsableItem {

    private CarUserControl controller;

    public override void SetOwner(GameObject newOwner)
    {
        owner = newOwner;

        if (owner.tag == "Player")
            controller = owner.GetComponent<CarUserControl>();
    }

    public override void use()
    {
        controller.Boost = boost;
        AudioManager.Instance.PlayTurbo(owner.GetComponents<AudioSource>()[1]);
        Destroy(gameObject, duration);
    }

    public void OnDestroy()
    {
        controller.Boost = 0f;
    }
}
