using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine.EventSystems;

public class LevelSelectorController : MonoBehaviour
{

    public List<LevelInfo> levels;
    public GameObject scrollViewContent;

    private List<GameObject> levelInfoObjects = new List<GameObject>();

    GameObject levelInfoPrefab;
    // Use this for initialization
    void Awake () {

        levelInfoPrefab = Resources.Load<GameObject>("Prefabs/LevelInfoObject");
        int i = 0;
        foreach (LevelInfo level in levels)
        {
            AddLevelInfo(level, i++);
        }
        
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    void AddLevelInfo(LevelInfo levelInfo, int index)
    {
        GameObject newLevelInfoObject = Instantiate(levelInfoPrefab);
        levelInfoObjects.Add(newLevelInfoObject);
        newLevelInfoObject.transform.SetParent(scrollViewContent.transform, false);
        newLevelInfoObject.transform.name = levelInfo.file;
        //newLevelInfoObject.transform.localPosition = new Vector3(0, -(levelInfoObjects.Count * 100));

        // Name
        newLevelInfoObject.transform.Find("Name").GetComponent<Text>().text = levelInfo.name;

        // File
        newLevelInfoObject.transform.Find("File").GetComponent<Text>().text = levelInfo.file;

        Button btn = newLevelInfoObject.GetComponent<Button>();
        btn.onClick.AddListener(() => {
            TrackManager.Instance.NbTrackLaps = levelInfo.lapCount;
            AudioManager.Instance.SetAudioTrack(levelInfo.clipName);
            LoadLevel(levelInfo.sceneID);
            
        });

        EventTrigger trigger = newLevelInfoObject.GetComponent<EventTrigger>();
        EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry();
        pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
        pointerEnterEntry.callback.AddListener((data) => { newLevelInfoObject.GetComponentInParent<NavigableMenu>().SetSelector(index); });
        trigger.triggers.Add(pointerEnterEntry);

        EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry();
        pointerExitEntry.eventID = EventTriggerType.PointerExit;
        pointerExitEntry.callback.AddListener((data) => { newLevelInfoObject.GetComponentInParent<NavigableMenu>().SetSelector(0); });
        trigger.triggers.Add(pointerExitEntry);

    }

    void LoadLevel(int sceneID)
    {
        GameManager.Instance.ChangeSceneWithLoadingScreen(sceneID);
    }

}

[Serializable]
public class LevelInfo
{
    LevelInfo(string name, string file, int sceneID, string clipName)
        { this.name = name; this.file = file; this.sceneID = sceneID; this.clipName = clipName; }
    public string name;
    public string file;
    public int sceneID;
    public int lapCount;
    public string clipName;
}