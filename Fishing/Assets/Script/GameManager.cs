using UnityEngine;
using UnityEngine.Localization.Settings;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set;}
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = -1;

        int lang = PlayerPrefs.GetInt("lang", 1);

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[lang];
    }
}
