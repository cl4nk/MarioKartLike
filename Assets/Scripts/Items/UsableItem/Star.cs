using Managers;
using UnityEngine;

namespace Items.UsableItem
{
    public class Star : UsableItem {

        private CarController controller;
        private CarCollisionController collCtl;

        public override void SetOwner(GameObject newOwner)
        {
            owner = newOwner;

            if (owner.tag == "Player")
            {
                controller = owner.GetComponent<CarController>();
                collCtl = owner.GetComponentInChildren<CarCollisionController>();
            }
        }

        public override void use()
        {
            controller.SetTopspeed(controller.MaxSpeed * 1.5f);
            collCtl.Invincible = true;
            AudioManager.Instance.PlayStarEffect(owner.GetComponents<AudioSource>()[1], duration);
            Destroy(gameObject, duration);
        }

        public void OnDestroy()
        {
            controller.UnsetTopspeed();
            collCtl.Invincible = false;
        }
    }
}
