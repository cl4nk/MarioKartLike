using UnityEngine;

namespace Managers
{
    public class BootManager : MonoBehaviour {

        [SerializeField] GameObject gameMgrPrefab;
        [SerializeField] GameObject guiMgrPrefab;
        [SerializeField]
        GameObject trackMgrPrefab;
        [SerializeField]
        GameObject musicSpkPrefab;
        [SerializeField]
        GameObject eventSpkPrefab;
        [SerializeField]
        GameObject audioMgrPrefab;
        static bool isLoaded = false;

        // Use this for initialization
        void Start () {
            if (!isLoaded)
            {
                GameObject gameMgr = Instantiate(gameMgrPrefab);
                GameObject guiMgr = Instantiate(guiMgrPrefab);
                GameObject trackMgr = Instantiate(trackMgrPrefab);
                GameObject audioMgr = Instantiate(audioMgrPrefab);
                GameObject musicSpk = Instantiate(musicSpkPrefab);
                musicSpk.name = "MusicSpeaker";
                GameObject eventSpk = Instantiate(eventSpkPrefab);
                eventSpk.name = "EventSpeaker";
                isLoaded = true;
                DontDestroyOnLoad(gameMgr);
                DontDestroyOnLoad(guiMgr);
                DontDestroyOnLoad(trackMgr);
                DontDestroyOnLoad(musicSpk);
                DontDestroyOnLoad(eventSpk);
                DontDestroyOnLoad(audioMgr);
            }
        }
    }
}
