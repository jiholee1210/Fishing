using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MouseManager : MonoBehaviour
{
    [SerializeField] private Slider sensitivity;
    [SerializeField] private CameraRot cameraRot;   
    [SerializeField] private TMP_Text valueText;
    private string tag = "Mouse";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DefaultSetting();
    }

    public void ChangeMouseSensitivity(float value) {
        SetMouseSenstivity(value);
    }

    private void SetMouseSenstivity(float value) {
        int input = (int)value;
        cameraRot.rotSpeed = value;
        valueText.text = input.ToString();
        PlayerPrefs.SetFloat(tag, value);
    }

    private void DefaultSetting() {
        int value = (int)PlayerPrefs.GetFloat(tag);
        sensitivity.value = value;
        valueText.text = value.ToString();
    }
}
