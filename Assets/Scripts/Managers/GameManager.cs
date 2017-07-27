using Items;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameManager : MonoBehaviour {

        private static GameManager instance;

        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<GameManager>();
                }

                return instance;
            }

        }

        public enum GameState
        {
            INTRO = 1,
            LOADING = 2,
            HIGHSCORE = 3,
            LEVELSELECTION = 4,
            LEVEL = 5
        }

        public GameState crtGameState = GameState.INTRO;

        public delegate void OnStateDelegate();
        public event OnStateDelegate OnIntro;
        public event OnStateDelegate OnHighscore;
        public event OnStateDelegate OnLevelSelection;
        public event OnStateDelegate OnLevel;
        public event OnStateDelegate ToIntro;
        public event OnStateDelegate ToHighscore;
        public event OnStateDelegate ToLevel;
        public event OnStateDelegate ToLevelSelector;
        public event OnStateDelegate OnGuiIntro;


        private int sceneId = 0;
        [SerializeField] private LoadingScreen loadingScreenPrefab;

        public Item[] itemPrefabs;

        // Use this for initialization
        void Start () {
            ToIntro += () => crtGameState = GameState.INTRO;
            ToHighscore += () => crtGameState = GameState.HIGHSCORE;
            ToLevel += () => crtGameState = GameState.LEVEL;
            ToLevelSelector += () => crtGameState = GameState.LEVELSELECTION;

            if (ToIntro != null)
                ToIntro();
        }
	
        // Update is called once per frame
        void Update () {
            switch (crtGameState)
            {
                case GameState.INTRO:
                {
                    if (OnIntro != null)
                        OnIntro();
                    break;
                }
                case GameState.HIGHSCORE:
                {
                    if (OnHighscore != null)
                        OnHighscore();
                    break;
                }
                case GameState.LEVELSELECTION:
                {
                    if (OnLevelSelection != null)
                        OnLevelSelection();
                    break;
                }
                case GameState.LEVEL:
                {
                    if (OnLevel != null)
                        OnLevel();
                    break;
                }
                default: break;
            }
        }

        void OnLevelWasLoaded()
        {
            // Not very flexible... 
            if (sceneId < 0)
                return;

            switch (sceneId)
            {
                case 0:
                {
                    if (ToIntro != null)
                        ToIntro();
                    break;
                }
                case 1:
                {
                    if (ToHighscore != null)
                        ToHighscore();
                    break;
                }
                case 2:
                {
                    if (ToLevelSelector != null)
                        ToLevelSelector();
                    break;
                }
                default:
                {
                    if (ToLevel != null)
                        ToLevel();
                    break;
                }
            }
        }

        void OnGUI()
        {
            if (crtGameState == GameState.INTRO)
                if (OnGuiIntro != null)
                    OnGuiIntro();
        }

        public void ChangeScene(string newScene)
        {
            SceneManager.LoadScene(newScene);
            sceneId = SceneManager.GetSceneByName(newScene).buildIndex;
        }

        public void ChangeScene(int newSceneId)
        {
            SceneManager.LoadScene(newSceneId);
            sceneId = newSceneId;
        }

        public void ChangeSceneWithLoadingScreen(string newScene)
        {
            sceneId = SceneManager.GetSceneByName(newScene).buildIndex;
            LoadingScreen loadingScreen = Instantiate(loadingScreenPrefab);
            loadingScreen.load(sceneId);
        }

        public void ChangeSceneWithLoadingScreen(int newSceneId)
        {
            sceneId = newSceneId;
            LoadingScreen loadingScreen = Instantiate(loadingScreenPrefab);
            loadingScreen.load(sceneId);
        }

        public void Quit()
        {
            Application.Quit();
        }

        public void ReloadTrack ()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            ChangeSceneWithLoadingScreen(sceneName);
        }

        public int GetItemIndex (string str)
        {
            for (int i = 0; i < itemPrefabs.Length; i++)
                if (str == itemPrefabs[i].Type)
                    return i;
            return -1;
        }
    }
}
