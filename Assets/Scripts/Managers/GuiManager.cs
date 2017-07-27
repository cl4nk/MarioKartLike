using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class GuiManager : MonoBehaviour {

        private static GuiManager instance;

        public static GuiManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<GuiManager>();
                }

                return instance;
            }

        }

        private Text timeLabel;
        private Text positionLabel;
        private Text lapLabel;
        private Image itemImage;
        private Text currentLapLabel;

        [SerializeField]
        Sprite[] itemSprites;
        private Sprite defaultSprite;

        Color[] colors;

    
    

        // Use this for initialization
        void Start()
        {
            colors = new Color[8] {
                Color.white,
                new Color(1f, 1f, 0.67f),
                new Color(.95f, .9f, .29f),
                new Color(.91f, .81f, 0),
                new Color(.9f, .67f, 0),
                new Color(.85f, .48f, .16f),
                new Color(.875f, .4f, .11f),
                new Color(.9f, .36f, .1f)};

            GameManager.Instance.ToLevel += () => StartLevel();
            GameManager.Instance.ToHighscore += () => LoadHighscore();

            GameObject hud = null, bsUI = null;
            TrackManager.Instance.OnTrackInit += () =>
            {
                hud = Instantiate(Resources.Load<GameObject>("Prefabs/HUDContainer"));
                hud.transform.SetParent(GameObject.Find("Canvas").transform, false);
            };

            TrackManager.Instance.OnTrackOutro += () =>
            {
                Destroy(hud);
                bsUI = Instantiate(Resources.Load<GameObject>("Prefabs/BestScoreUI"));
                bsUI.transform.SetParent(GameObject.Find("Canvas").transform, false);
                bsUI.GetComponent<HighScoreUI>().OnDestroyAction += () =>
                {
                    GameObject outro = Instantiate(Resources.Load<GameObject>("Prefabs/TrackOutro"));
                    outro.transform.SetParent(GameObject.Find("Canvas").transform, false);
                };

            };

        }

        private void StartLevel()
        {
            timeLabel = GameObject.Find("TimeLabel").GetComponent<Text>();
            positionLabel = GameObject.Find("PositionLabel").GetComponent<Text>();
            lapLabel = GameObject.Find("LapLabel").GetComponent<Text>();
            currentLapLabel = GameObject.Find("CurrentLapLabel").GetComponent<Text>();
            //itemLabel = GameObject.Find("ItemLabel").GetComponent<Text>();
            itemImage = GameObject.Find("ItemImage").GetComponent<Image>();
            defaultSprite = itemImage.sprite;

            TrackManager.Instance.OnTimeChange += ChangeTime;
            TrackManager.Instance.OnPosPlayerChange += ChangePosition;
            TrackManager.Instance.OnPlayerLapCountChange += ChangeLap;
            TrackManager.Instance.OnPlayerLapCountChange += ChangeCurrentLap;

        }

        private void LoadHighscore()
        { }

        public void ClickOnLaunchButton()
        {
            GameManager.Instance.ChangeScene("Scenes/LevelSelector");
        }

        public void ClickOnHighscoreButton()
        {
            GameManager.Instance.ChangeScene(1);
        }

        public void ClickOnQuitButton()
        {
            GameManager.Instance.Quit();
        }

        public void ClickOnBackToMenuButton()
        {
            GameManager.Instance.ChangeScene(0);
        }

        public void ClickRetryButton ()
        {
            GameManager.Instance.ReloadTrack();
        }

        public void ChangeTime(float time)
        {
            if (timeLabel)
            {
                int minutes = Mathf.FloorToInt(time / 60);
                float secondes = (time % 60);
                timeLabel.text = "Time : " + minutes.ToString("00") + ":" + secondes.ToString("00.00");
            }
        }

        public void ChangePosition(int pos)
        {
            if (positionLabel)
            {
                positionLabel.text = "#" + pos;
                positionLabel.GetComponent<Text>().color = colors[pos - 1];
                positionLabel.GetComponent<Animation>().Play();

            }
        }

        public void ChangeLap(int lap, int trackLap)
        {
            if (lapLabel)
            {
                if (lap <= trackLap)
                    lapLabel.text = "Lap : " + lap + " / " + trackLap;
            }
        }

        public void ChangeCurrentLap(int lap, int trackLap)
        {
            if (currentLapLabel)
            {
                if (lap == 1)
                    return;
                if (lap == trackLap)
                    currentLapLabel.text = "last lap";
                else if (lap <= trackLap)
                    currentLapLabel.text = "lap " + lap;
                currentLapLabel.GetComponent<Animation>().Play();
            }
        }

        public void ChangeItem(string item)
        {
            if (itemImage)
                StartCoroutine(ItemImageCoroutine(GameManager.Instance.GetItemIndex(item)));

        }

        IEnumerator ItemImageCoroutine (int index)
        {
            if (itemImage == null)
                yield break;
            if (index == -1)
            {
                itemImage.sprite = defaultSprite;
                yield break;
            }

            AudioManager.Instance.PlayRoulette();

            int count = 20;
            float time = 1;
            for (int i = 0; i < count; i++)
            {
                if (itemImage == null)
                    yield break;
                itemImage.sprite = itemSprites[i % itemSprites.Length];
                yield return new WaitForSeconds(time / count);
            }
            for (int i = 0; i < count; i++)
            {
                if (itemImage == null)
                    yield break;
                itemImage.sprite = itemSprites[i % itemSprites.Length];
                yield return new WaitForSeconds((time / count) * 2);
            }
            if (itemImage == null)
                yield break;
            itemImage.sprite = itemSprites[index];
            AudioManager.Instance.PlayGotItem();

        }


    }
}