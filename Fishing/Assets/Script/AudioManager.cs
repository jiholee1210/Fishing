using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public enum AudioMixerType {
    Master,
    BGM,
    SFX
}
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private TMP_Text[] volText;
    [SerializeField] private Slider[] volSliders;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DefaultSetting();
    }

    public void ChangeMasterVolume(float vol) {
        SetAudioVol(AudioMixerType.Master, vol);
    }

    public void ChangeBGMVolume(float vol) {
        SetAudioVol(AudioMixerType.BGM, vol);
    }

    public void ChangeSFXVolume(float vol) {
        SetAudioVol(AudioMixerType.SFX, vol);
    }

    private void SetAudioVol(AudioMixerType audioMixerType, float vol) {
        string volTag = audioMixerType.ToString();
        int uiVol = (int)Mathf.Clamp(vol * 100f, 0, 100);
        int value = (int)audioMixerType;
        volText[value].text = uiVol.ToString();

        PlayerPrefs.SetFloat(volTag, vol);
        audioMixer.SetFloat(audioMixerType.ToString(), Mathf.Log10(vol) * 20);
    }

    private void DefaultSetting() {
        for(int i = 0; i < 3; i++) {
            AudioMixerType audioMixerType = (AudioMixerType)i;

            string tag = audioMixerType.ToString();
            float vol = PlayerPrefs.GetFloat(tag);

            volSliders[i].value = vol;
            volText[i].text = ((int)Mathf.Clamp(vol * 100f, 0, 100)).ToString();
        }

    }
}
