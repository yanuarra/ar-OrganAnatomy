using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {
    //[SerializeField] private CacheData cacheData;

    public static SceneLoader Instance { get; private set; }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(this);
        }
    }

    public void LoadScene(string sceneName) {
        if (!Application.CanStreamedLevelBeLoaded(sceneName)) {
            Debug.LogWarning("Scene name is not available");
            return;
        }
        //cacheData.sceneToLoad = sceneName;
        //LoadLoadingScene();
        SceneManager.LoadScene(sceneName);
    }

    private void LoadLoadingScene() {
        SceneManager.LoadScene("Loading");
    }
}
