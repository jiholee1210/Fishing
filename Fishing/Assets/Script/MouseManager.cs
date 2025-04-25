using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MouseManager : MonoBehaviour
{
    [SerializeField] private Slider sensitivity;
    [SerializeField] private CameraRot cameraRot;   
    [SerializeField] private TMP_Text valueText;
    private new readonly string tag = "Mouse";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DefaultSetting();
    }

    public void ChangeMouseSensitivity(float value) {
        SetMouseSenstivity(value);
    }

    private void SetMouseSenstivity(float value) {
        float input = value;
        cameraRot.rotSpeed = value;
        valueText.text = input.ToString("F2");
        PlayerPrefs.SetFloat(tag, value);
    }

    public void DefaultSetting() {
        float value = PlayerPrefs.GetFloat(tag);
        sensitivity.value = value;
        cameraRot.rotSpeed = value;
        valueText.text = value.ToString("F2");
    }
}
