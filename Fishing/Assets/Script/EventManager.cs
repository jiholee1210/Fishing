using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set;}

    [SerializeField] PlayerActing playerActing;
    [SerializeField] UIManager uIManager;
    [SerializeField] GameObject npcWindow;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;
    }

    public void StartFishing(List<FishData> fishList) {
        uIManager.OpenFishingUI(fishList);
    }

    public void EndFishing() {
        playerActing.EndFishing();
    }

    public void OpenInventory() {
        uIManager.OpenInventoryUI();
    }

    public void CloseInventory() {
        uIManager.CloseInventoryUI();
    }

    public void OpenFishInventory() {
        uIManager.OpenFishInventoryUI();
    }

    public void CloseFishInventory() {
        uIManager.CloseFishInventoryUI();
    }

    public void OpenQuest() {
        uIManager.OpenQuestUI();
    }

    public void CloseQuest() {
        uIManager.CloseQuestUI();
    }

    public void OpenGuide() {
        uIManager.OpenGuideUI();
    }

    public void CloseGuide() {
        uIManager.CloseGuideUI();
    }

    public void OpenNPCUI(int npcType, GameObject npcObject) {
        npcWindow.SetActive(true);
        uIManager.SetNpcObject(npcObject);
        switch(npcType) {
            case 1:
                uIManager.OpenEquipMerchantTalkUI();
                break;
            case 2:
                uIManager.OpenFishMerchantTalkUI();
                break;
            case 3:
                uIManager.OpenQuestNpcTalkUI();
                break;
        } 
    }

    public void CloseNpcUI(int npcType) {
        Debug.Log(npcType);
        switch(npcType) {
            case 1:
                uIManager.CloseEquipMerchantTalkUI();
                break;
            case 2:
                uIManager.CloseFishMerchantTalkUI();
                break;
            case 3:
                uIManager.CloseQuestNpcTalkUI();
                break;
        }
        npcWindow.SetActive(false);
    }

    public void CloseAllWindows() {
        npcWindow.SetActive(false);
        uIManager.CloseAllWindows();
    }
}
