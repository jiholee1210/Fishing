using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private Button confirmButton;
    void Start()
    {
        confirmButton.onClick.AddListener(() => CloseTutorial());
    }

    private void CloseTutorial() {
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
