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


    public void ClickSound() {
        UI.PlayOneShot(clickUI);
    }

    public void ActingFailSound() {
        UI.PlayOneShot(actingFail);
    }

    private IEnumerator SwingWithDelay(float delay) {
        yield return new WaitForSeconds(delay);
        UI.PlayOneShot(swing);
    }
    public void SwingRod() {
        StartCoroutine(SwingWithDelay(0.3f));
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
