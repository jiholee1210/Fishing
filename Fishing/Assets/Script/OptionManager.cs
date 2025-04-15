using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    [SerializeField] private Button soundButton;
    [SerializeField] private Button mouseButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button soundConfirmButton;
    [SerializeField] private Button mouseConfirmButton;

    void Start()
    {
        soundButton.onClick.AddListener(() => OpenVolumeSetting());
        mouseButton.onClick.AddListener(() => OpenMouseSetting());
        exitButton.onClick.AddListener(() => EventManager.Instance.SaveAndExit());
        soundConfirmButton.onClick.AddListener(() => CloseVolumeButton());
        mouseConfirmButton.onClick.AddListener(() => CloseMouseButton());
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

    private void OpenMouseSetting() {
        SoundManager.Instance.ButtonClick();
        transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
    }

    private void CloseMouseButton() {
        CloseMouseSetting();
        SoundManager.Instance.ButtonClick();
    }

    private void CloseMouseSetting() {
        transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
    }

    public void CloseWindow() {
        CloseVolumeSetting();
        CloseMouseSetting();
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
