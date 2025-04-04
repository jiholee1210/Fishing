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
        } 
    }

    public void CloseNpcUI(int npcType) {
        switch(npcType) {
            case 1:
                uIManager.CloseUpgradeNpcUI();
                break;
            case 2:
                uIManager.CloseFishMerchantUI();
                break;
            case 3:
                uIManager.CloseQuestNpcUI();
                break;
            case 4:
                uIManager.CloseFishFarmNpcUI();
                break;
        }
        playerActing.EndTalk();
    }

    public void OpenSignUI() {
        uIManager.OpenSignUI();
    }

    public void CloseSignUI() {
        uIManager.CloseSignUI();
        playerActing.EndTalk();
    }

    public void OpenOptionUI() {
        uIManager.OpenOptionUI();
    }

    public void CloseAllWindows() {
        uIManager.CloseAllWindows();
    }

    public void ClearQuest(int id) {
        switch(id) {
            case 1:
                Debug.Log("양식장 해금");
                DataManager.Instance.playerData.farmUnlock[0] = true;
                DataManager.Instance.playerData.farmUnlock[1] = true;
                DataManager.Instance.SavePlayerData();
                break;
            case 2:
                Debug.Log("박물관 해금");
                break;
            case 3:
                Debug.Log("바위지대 해금");
                break;
            case 4:
                Debug.Log("용암지대 해금");
                break;
            case 5:
                Debug.Log("바위지대 양식 해금");
                DataManager.Instance.playerData.farmUnlock[2] = true;
                break;
            case 6:
                Debug.Log("용암지대 양식 해금");
                DataManager.Instance.playerData.farmUnlock[3] = true;
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

        SceneChanger.Instance.BackToMain();
    }
}
