using System.Collections;
using Managers;
using UnityEngine;

namespace Items
{
    public class ItemManager : MonoBehaviour {

        Item item;
        [SerializeField] Item itemInst;
        bool tripleWasInst = false;

        public delegate void OnLaunchDelegate();
        public event OnLaunchDelegate OnDefaultLaunch;

        private enum DropState
        {
            Default = 0,
            Forward = 1,
            Backward = 2
        }

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetButtonDown("Jump"))
            {
                if (item)
                {
                    if (itemInst == null && item.tag == "LaunchableItem")
                        dragItem();
                    else
                        useItem();
                }
            }

            if (itemInst && itemInst.tag == "LaunchableItem")
            {
                if (Input.GetButton("Vertical") && Input.GetAxisRaw("Vertical") > 0 && Input.GetButtonUp("Jump"))
                {
                    if (!CheckIfTripleWasInst())
                        dropItem(DropState.Forward);
                    return;
                }

                if (Input.GetButton("Vertical") && Input.GetAxisRaw("Vertical") < 0 && Input.GetButtonUp("Jump"))
                {
                    if (!CheckIfTripleWasInst())
                        dropItem(DropState.Backward);
                    return;
                }

                if (Input.GetButtonUp("Jump"))
                {
                    if (!CheckIfTripleWasInst())
                        dropItem(DropState.Default);
                }
            }
        }

        private bool CheckIfTripleWasInst()
        {
            if (tripleWasInst)
            {
                tripleWasInst = false;
                return true;
            }

            return false;
        }

        private bool instantiateItem()
        {
            if (item && !itemInst)
            {
                itemInst = Instantiate(item);
                itemInst.SetOwner(gameObject);
                item = null;
                if (itemInst.Type == "TripleRedShell" || itemInst.Type == "TripleGreenShell")
                    tripleWasInst = true;

                return true;
            }

            return false;
        }

        private void dragItem()
        {
            if (instantiateItem())
            {
                itemInst.transform.position = gameObject.transform.position + new Vector3(0, 1f, 3);
            }
        }

        private void dropItem(DropState state)
        {
            if (itemInst == null)
                return;

            switch (state)
            {
                case DropState.Forward:
                {
                    itemInst.GetComponent<LaunchableItem.LaunchableItem>().LaunchForward();
                    break;
                }
                case DropState.Backward:
                {
                    itemInst.GetComponent<LaunchableItem.LaunchableItem>().LaunchBackward();
                    break;
                }
                case DropState.Default:
                {

                    OnDefaultLaunch();
                    AudioManager.Instance.PlayThrow(GetComponents<AudioSource>()[1]);
                    break;
                }
                default : { break; }
            }

            if (itemInst && itemInst.Type != "TripleRedShell" && itemInst.Type != "TripleGreenShell")
            {
                itemInst = null;

                if (GuiManager.Instance)
                    GuiManager.Instance.ChangeItem("");
            }
        }

        private void useItem()
        {
            if (instantiateItem())
            {
                itemInst.GetComponent<UsableItem.UsableItem>().use();

                itemInst = null;

                if (GuiManager.Instance)
                    GuiManager.Instance.ChangeItem("");
            }
        }

        public void setItem(Item newItem)
        {
            if (!item)
            {
                StartCoroutine(RouletteCoroutine(newItem));
            }
        }

        IEnumerator RouletteCoroutine (Item newItem)
        {
            if (gameObject.name != "Player")
                yield break;
            if (GuiManager.Instance)
                GuiManager.Instance.ChangeItem(newItem.Type);
            yield return new WaitForSeconds(3);
            item = newItem;
        }

        public void destroyTriple()
        {
            Destroy(itemInst.gameObject);

            itemInst = null;

            if (GuiManager.Instance)
                GuiManager.Instance.ChangeItem("");
        }
    }
}
