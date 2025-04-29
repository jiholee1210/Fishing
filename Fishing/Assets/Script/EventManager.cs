using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set;}

    [SerializeField] PlayerActing playerActing;
    [SerializeField] UIManager uIManager;

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

    public void OpenFishInventory() {
        uIManager.OpenFishInventoryUI();
    }

    public void CloseFishInventory() {
        uIManager.CloseFishInventoryUI();
    }

    public void OpenGuide() {
        uIManager.OpenGuideUI();
    }

    public void CloseGuide() {
        uIManager.CloseGuideUI();
    }

    public void OpenSkin() {
        uIManager.OpenSkinUI();
    }

    public void CloseSkin() {
        uIManager.CloseSkinUI();
    }

    public void OpenNPCUI(int npcType, GameObject npcObject) {
        uIManager.SetNpcObject(npcObject);
        switch(npcType) {
            case 1:
                uIManager.OpenUpgradeNpcUI();
                break;
            case 2:
                uIManager.OpenFishMerchantUI();
                break;
            case 3:
                uIManager.OpenQuestNpcUI();
                break;
            case 4:
                uIManager.OpenFishFarmNpcUI();
                break;
            case 5:
                uIManager.OpenMuseumNpcUI();
                break;
            case 6:
                uIManager.OpenEndingNpcUI();
                break;
        } 
    }

    public void OpenOptionUI() {
        uIManager.OpenOptionUI();
    }

    public void InventoryFull() {
        uIManager.InventoryFull();
    }

    public void NotClearQuest() {
        uIManager.NotClearQuest();
    }

    public void ReqFish() {
        uIManager.ReqFish();
    }

    public void SelectFish() {
        uIManager.SelectFish();
    }

    public void DontHaveFish() {
        uIManager.DontHaveFish();
    }

    public void NotEnoughGold() {
        uIManager.NotEnoughGold();
    }

    public void SelectRelic() {
        uIManager.SelectRelic();
    }

    public void ShowTutorial() {
        uIManager.OpenTutorial();
    }

    public void CloseAllWindows() {
        uIManager.CloseAllWindows();
    }
    
    public void ClearQuest(int id) {
        switch(id) {
            case 1:
                DataManager.Instance.playerData.farmUnlock[0] = true;
                DataManager.Instance.playerData.farmUnlock[1] = true;
                DataManager.Instance.SavePlayerData();
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                DataManager.Instance.playerData.farmUnlock[2] = true;
                DataManager.Instance.SavePlayerData();
                break;
            case 6:
                DataManager.Instance.playerData.farmUnlock[3] = true;
                DataManager.Instance.SavePlayerData();
                break;
        }
    }

    public void SaveAndExit() {
        Vector3 playerPos = playerActing.GetPos();
        DataManager.Instance.playerData.pos = playerPos;
        DataManager.Instance.SavePlayerData();
        DataManager.Instance.SaveGuideData();
        DataManager.Instance.SaveInventoryData();
        DataManager.Instance.SaveQuestNpcData();

        SceneChanger.Instance.GameExit();
    }
}
