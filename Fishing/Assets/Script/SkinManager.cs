using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkinManager : MonoBehaviour
{
    [SerializeField] private GameObject[] buttons;
    [SerializeField] private Transform handPos;
    [SerializeField] private GameObject[] rodPrefab;
    [SerializeField] private TooltipManager tooltipManager;
    [SerializeField] private PlayerActing playerActing;

    private List<int> rodList;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DefaultSetting();
    }

    public void SetSlots() {
        for(int i = 0; i < buttons.Length; i++) {
            int index = i;
            buttons[index].GetComponent<RodSlot>().slotID = index;
            buttons[index].GetComponent<Button>().onClick.RemoveAllListeners();

            if(rodList.Contains(index)) {
                if(DataManager.Instance.playerData.curRod == index) {
                    buttons[index].GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                    buttons[index].transform.GetChild(0).GetComponent<TMP_Text>().text = "착용 중";
                    buttons[index].GetComponent<Button>().enabled = false;
                    continue;
                }
                buttons[index].GetComponent<Image>().color = new Color(1f, 1f, 1f);
                buttons[index].GetComponent<Button>().enabled = true;
                buttons[index].GetComponent<Button>().onClick.AddListener(() => ChangeRodSkin(index));
                buttons[index].transform.GetChild(0).GetComponent<TMP_Text>().text = "변경";
                buttons[index].transform.GetChild(0).GetComponent<TMP_Text>().color = new Color(0.3f, 0.1f, 0f);
            }
            else {
                buttons[index].GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f);
                buttons[index].GetComponent<Button>().enabled = false;
                buttons[index].transform.GetChild(0).GetComponent<TMP_Text>().text = "사용 불가";
                buttons[index].transform.GetChild(0).GetComponent<TMP_Text>().color = new Color(0f, 0f, 0f);
            }
        }
    }

    private void ChangeRodSkin(int index) {
        SoundManager.Instance.SkinChange();
        if(handPos.childCount > 0) {
            Destroy(handPos.GetChild(0).gameObject);
        }
        DataManager.Instance.playerData.curRod = index;
        Instantiate(rodPrefab[index], handPos);
        StartCoroutine(playerActing.SetAnimator());

        DataManager.Instance.SavePlayerData();
        SetSlots();
    }

    private void DefaultSetting() {
        rodList = DataManager.Instance.playerData.rodList;
    }
    
    public void CloseWindow() {
        for(int i = 0; i < buttons.Length; i++) {
            tooltipManager.HideRod(i);
        }
    }
}
