using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    [SerializeField] private Button optionButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button confirmButton;

    void Start()
    {
        optionButton.onClick.AddListener(() => OpenVolumeSetting());
        exitButton.onClick.AddListener(() => EventManager.Instance.SaveAndExit());
        confirmButton.onClick.AddListener(() => CloseVolumeButton());
    }

    private void OpenVolumeSetting() {
        SoundManager.Instance.ButtonClick();
        transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
    }

    private void CloseVolumeButton() {
        CloseVolumeSetting();
        SoundManager.Instance.ButtonClick();
    }

    private void CloseVolumeSetting() {
        transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
    }

    public void CloseWindow() {
        CloseVolumeSetting();

        transform.GetChild(0).gameObject.SetActive(false);
    }
}
