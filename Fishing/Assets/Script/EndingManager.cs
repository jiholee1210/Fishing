using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class EndingManager : MonoBehaviour
{
    [SerializeField] private Button yes;
    [SerializeField] private Button main;
    [SerializeField] private TMP_Text guide;

    [SerializeField] private WipeController wipeController;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerActing playerActing;

    [SerializeField] private Animator picAnimator;
    [SerializeField] private Animator endAnimator;

    private LocalizedString guideString = new LocalizedString("DialogTable", "ending_guide");

    void Start()
    {
        yes.onClick.AddListener(() => StartCoroutine(StartEnding()));
        main.onClick.AddListener(() => GoToMain());
    }

    private IEnumerator StartEnding() {
        if (DataManager.Instance.playerData.gold < 100000) {
            SoundManager.Instance.ActingFailSound();
            EventManager.Instance.NotEnoughGold();
            yield break;
        }
        SoundManager.Instance.ButtonClick();
        playerMovement.LockControl();
        playerActing.SetEndingState();
        yield return StartCoroutine(wipeController.CircleIn());
        yield return new WaitForSeconds(1f);
        transform.GetChild(0).gameObject.SetActive(false);
        playerMovement.gameObject.transform.position = new Vector3(1275.5f, -75.2f, 1921.3f);
        transform.GetChild(1).gameObject.SetActive(true);
        CalGuidePercent();
        yield return StartCoroutine(wipeController.CircleOut());
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(OpenPic());
        yield return StartCoroutine(OpenEnding());
    }

    private IEnumerator OpenPic() {
        picAnimator.Play("Open_Pic");
        yield return null;

        float len = picAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(len + 1f);
    }

    private IEnumerator OpenEnding() {
        endAnimator.Play("Open_Ending");
        yield return null;

        float len = endAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(len);
    }

    private void CalGuidePercent() {
        List<bool> caughtFish = DataManager.Instance.guide.fishID;
        int count = 0;
        for(int i = 0; i < caughtFish.Count; i++) {
            if(caughtFish[i]) {
                count += DataManager.Instance.guide.fishGrade[i].grade.Count(b => b);
            }
        }
        int percent = (int)(count / 160f * 100f);
        guideString.Arguments = new object[] {percent};
        guide.text = guideString.GetLocalizedString();
    }

    private void GoToMain() {
        EventManager.Instance.SaveAndExit();
    }
}
