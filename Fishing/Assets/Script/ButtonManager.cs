using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button endButton;
    [SerializeField] private Button languageButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button korean;
    [SerializeField] private Button english;

    [SerializeField] private Transform languagePanel;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startButton.onClick.AddListener(() => SceneChanger.Instance.GameStart());
        endButton.onClick.AddListener(() => SceneChanger.Instance.ExitGame());
        languageButton.onClick.AddListener(() => OpenLanguage());
        backButton.onClick.AddListener(() => CloseLanguage());
        korean.onClick.AddListener(() => SetLanguage(0));
        english.onClick.AddListener(() => SetLanguage(1));
    }

    private void OpenLanguage() {
        languagePanel.gameObject.SetActive(true);
    }

    private void CloseLanguage() {
        languagePanel.gameObject.SetActive(false);
    }

    private void SetLanguage(int id) {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[id];
        PlayerPrefs.SetInt("lang", id);
        CloseLanguage();
    }
}
