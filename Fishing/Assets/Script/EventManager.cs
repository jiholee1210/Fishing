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

    public void OpenNPCUI(int npcType, GameObject npcObject) {
        uIManager.SetNpcObject(npcObject);
        switch(npcType) {
            case 1:
                uIManager.OpenUpgradeNpcTalkUI();
                break;
            case 2:
                uIManager.OpenFishMerchantTalkUI();
                break;
            case 3:
                uIManager.OpenQuestNpcTalkUI();
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
                uIManager.CloseUpgradeNpcTalkUI();
                break;
            case 2:
                uIManager.CloseFishMerchantTalkUI();
                break;
            case 3:
                uIManager.CloseQuestNpcTalkUI();
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

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
