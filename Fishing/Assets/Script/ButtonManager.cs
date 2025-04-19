using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button endButton;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startButton.onClick.AddListener(() => SceneChanger.Instance.GameStart());
        endButton.onClick.AddListener(() => SceneChanger.Instance.ExitGame());
    }
}
