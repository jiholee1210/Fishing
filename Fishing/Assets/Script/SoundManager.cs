using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource BGM;
    [SerializeField] private AudioSource UI;
    [SerializeField] private AudioSource reel;
    [SerializeField] private AudioSource walk;

    [SerializeField] private AudioClip clickUI;
    [SerializeField] private AudioClip actingFail;
    [SerializeField] private AudioClip swing;
    [SerializeField] private AudioClip noteSuccess;
    [SerializeField] private AudioClip noteFail;
    [SerializeField] private AudioClip fishingSuccess;
    [SerializeField] private AudioClip fishingFail;
    [SerializeField] private AudioClip fishingDetail;
    [SerializeField] private AudioClip upgradeClick;


    public void ClickSound() {
        UI.PlayOneShot(clickUI);
    }

    public void ActingFailSound() {
        UI.PlayOneShot(actingFail);
    }

    public void UpgradeClick() {
        UI.PlayOneShot(upgradeClick);
    }

    private IEnumerator SwingWithDelay(float delay) {
        yield return new WaitForSeconds(delay);
        UI.PlayOneShot(swing);
    }
    public void SwingRod() {
        StartCoroutine(SwingWithDelay(0.3f));
    }

    public void NoteSuccess() {
        UI.PlayOneShot(noteSuccess);
    }

    public void NoteFail() {
        UI.PlayOneShot(noteFail);
    }

    public void FishingSuccess() {
        UI.PlayOneShot(fishingSuccess);
    }
    
    public void FishingFail() {
        UI.PlayOneShot(fishingFail);
    }

    public void FishingDetail() {
        UI.PlayOneShot(fishingDetail);
    }

    public void PlayReelSound() {
        reel.Play();
    }

    public void StopReelSound() {
        reel.Stop();
    }

    public void PlayWalkingSound() {
        walk.Play();
    }

    public void StopWalkingSound() {
        walk.Stop();
    }
}
