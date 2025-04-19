using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance { get; private set; }
    [SerializeField] private WipeController wipeController;
    
    private string gameScene = "GameScene";
    private string mainScene = "MainScene";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    public void GameStart() {
        StartCoroutine(StartSequence());
    }

    private IEnumerator StartSequence() {
        yield return StartCoroutine(wipeController.CircleIn());
        SceneManager.LoadScene(gameScene);
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(wipeController.CircleOut());
    }

    public void BackToMain() {
        SceneManager.LoadScene(mainScene);
    }

    public void ExitGame() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
