using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance { get; private set; }

    private string gameScene = "GameScene";
    private string mainScene = "MainScene";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    public void GameStart() {
        SceneManager.LoadScene(gameScene);
    }

    public void OpenOption() {
        
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
