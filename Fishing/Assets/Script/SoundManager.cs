using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set;}

    [SerializeField] private AudioSource BGM;
    [SerializeField] private AudioSource UI;
    [SerializeField] private AudioSource reel;
    [SerializeField] private AudioSource walk;

    [SerializeField] private AudioClip openUI;
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip actingFail;
    [SerializeField] private AudioClip swing;
    [SerializeField] private AudioClip noteSuccess;
    [SerializeField] private AudioClip noteFail;
    [SerializeField] private AudioClip fishBite;
    [SerializeField] private AudioClip fishingSuccess;
    [SerializeField] private AudioClip fishingFail;
    [SerializeField] private AudioClip fishingDetail;
    [SerializeField] private AudioClip upgradeClick;
    [SerializeField] private AudioClip sellFish;
    [SerializeField] private AudioClip fishfarmSet;
    [SerializeField] private AudioClip skinChange;
    [SerializeField] private AudioClip questClear;
    [SerializeField] private AudioClip questFail;
    

    void Start()
    {
        Instance = this;
    }

    public void OpenUI() {
        UI.PlayOneShot(openUI);
    }

    public void ButtonClick() {
        UI.PlayOneShot(buttonClick);
    }

    public void ActingFailSound() {
        UI.PlayOneShot(actingFail);
    }

    public void UpgradeClick() {
        UI.PlayOneShot(upgradeClick);
    }

    public void SellFish() {
        UI.PlayOneShot(sellFish);
    }

    public void FishfarmSet() {
        UI.PlayOneShot(fishfarmSet);
    }

    public void SkinChange() {
        UI.PlayOneShot(skinChange);
    }

    public void QuestClear() {
        UI.PlayOneShot(questClear);
    }

    public void QuestFail() {
        UI.PlayOneShot(questFail);
    }

    public void FishBite() {
        UI.PlayOneShot(fishBite);
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
