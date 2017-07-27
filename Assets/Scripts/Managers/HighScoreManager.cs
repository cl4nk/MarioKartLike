using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

class HighScoreManager
{
    List<float> bestTimes;
    int trackId;

    //public HighScoreManager(string name) : this(SceneManager.GetSceneByPath("Scenes/Levels/" + name).buildIndex) { }

    public HighScoreManager(int trackId)
    {
        bestTimes = new List<float>();
        this.trackId = trackId;

        float currentTime;
        int position = 0;

        while ((currentTime = PlayerPrefs.GetFloat("Track " + trackId + " " + position)) != 0.0f)
        {
            bestTimes.Add(currentTime);
            position++;
        }
    }

    //return true if is in the 50 best scores 
    public bool AddScore(float time)
    {
        int btCount = bestTimes.Count;
        if (btCount >= 50 && bestTimes[btCount - 1] < time)
            return false;
        bestTimes.Add(time);
        bestTimes.Sort();
        for (int i = 0; i < Mathf.Min(bestTimes.Count, 50); i++)
        {
            PlayerPrefs.SetFloat("Track " + trackId + " " + i, bestTimes[i]);
        }
        return true;
    }

    public List<float> GetListScores()
    {
        return bestTimes;
    }
}
