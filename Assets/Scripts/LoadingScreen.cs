using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour {

    private bool animCoroutineFinish = true;
    private Text loadingText;

	// Use this for initialization
	void Start () {
        loadingText = GameObject.Find("LoadingText").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        if (animCoroutineFinish)
            StartCoroutine(AnimCoroutine());
	}

    public void load(int sceneId)
    {
        StartCoroutine(LoadNewLevel(sceneId));
    }

    IEnumerator AnimCoroutine()
    {
        animCoroutineFinish = false;
        loadingText.text = "Loading .";
        yield return new WaitForSeconds(1);
        loadingText.text = "Loading ..";
        yield return new WaitForSeconds(1);
        loadingText.text = "Loading ...";
        yield return new WaitForSeconds(1);
        animCoroutineFinish = true;
    }

    IEnumerator LoadNewLevel(int sceneId)
    {
        yield return new WaitForSeconds(2);

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneId);

        while (!async.isDone)
        {
            yield return null;
        }

        Destroy(gameObject);
    }
}
