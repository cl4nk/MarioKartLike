using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HighScoreUI : MonoBehaviour {


    public delegate void SimpleDelegate();
    public event SimpleDelegate OnDestroyAction;
    public GameObject item;
    

    public bool auto;
    public int sceneId;

    // Use this for initialization
    void Start () {
        HighScoreManager mgr;
        if (auto)
            mgr = new HighScoreManager(SceneManager.GetActiveScene().buildIndex);
        else
            mgr = new HighScoreManager(sceneId);
        List <float> highscores = mgr
            .GetListScores();
        PopulateList(highscores);
        //DirtyPopulateList();
	}

    void DirtyPopulateList ()
    {
        List<float> scores = new List<float>();
        for (int i = 0; i < 5; i++)
        {
            scores.Add(Random.Range(0, 300));
        }
        PopulateList(scores);
    }

    void PopulateList (List<float> scores)
    {
        Debug.Log("Count " + scores.Count);
        Color color = Color.clear;
        Transform container = transform
            .Find("Scroll View")
            .Find("Viewport")
            .Find("Content");

        for (int i = 0; i < scores.Count; i++)
        {
            GameObject o = Instantiate(item) as GameObject;
            o.transform.SetParent(container);
            Text[] texts = o.GetComponentsInChildren<Text>();
            foreach (Text txt in texts)
            {
                if (txt == null)
                    continue;
                
                switch (i)
                {
                    case 0:
                        color = Color.yellow;
                        break;
                    case 1:
                        color = Color.gray;
                        break;
                    case 2:
                        color = Color.red;
                        break;
                    default:
                        color = Color.clear;
                        break;
                }

                txt.GetComponentInChildren<Outline>().effectColor = color;

                if (txt.text == "POS")
                    txt.text = "#" + (i + 1);
                if (txt.text == "TIME")
                    txt.text = "TIME : " + ConvertTimeToString(scores[i]);

            }
        }
    }

    string ConvertTimeToString(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        float secondes = (time % 60);
        return minutes.ToString("00") + ":" + secondes.ToString("00.00");
    }

    void OnDestroy()
    {
        if (OnDestroyAction != null)
            OnDestroyAction();
    }

    public void OnBackClicked()
    {
        Destroy(gameObject);
    }
}
