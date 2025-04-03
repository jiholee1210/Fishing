using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] Transform[] equipType;
    [SerializeField] TMP_Text goldText;
    [SerializeField] PlayerInventory playerInventory;
    [SerializeField] TooltipManager tooltipManager;

    private int[] equipList = new int[5];
    private PlayerData playerData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        equipList = DataManager.Instance.inventory.equip;
        playerData = DataManager.Instance.playerData;
    }

    //각 인덱스별 값에 따라 이미지 오픈, 버튼 오브젝트 활성화
    public void DefaultSetting() {
        for(int i = 0; i < equipList.Length; i++) {
            int indexA = i;

            for(int j = 0; j <= equipList[indexA]; j++) {
                int indexB = j;
                if(indexB != 0) {
                    equipType[indexA].GetChild(indexB).GetChild(1).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                }
                if(indexB != 3 && indexB == equipList[indexA]) {
                    SetBuyButton(indexA, indexB + 1);
                }
            }
        }
        SetGoldText();
    }

    private void SetBuyButton(int type, int level) {
        equipType[type].GetChild(level).GetChild(0).gameObject.SetActive(true);
        equipType[type].GetChild(level).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = DataManager.Instance.GetItemData(type * 10 + level).reqGold + " C";
        equipType[type].GetChild(level).GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
        Debug.Log(playerData.gold + " " + DataManager.Instance.GetItemData(type * 10 + level).reqGold);
        if(playerData.gold >= DataManager.Instance.GetItemData(type * 10 + level).reqGold) {
            Debug.Log("진입1");
            equipType[type].GetChild(level).GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f);
            equipType[type].GetChild(level).GetChild(0).GetComponent<Button>().onClick.AddListener(() => UpgradeEquip(type, level));
        }
        else {
            Debug.Log("진입2");
            equipType[type].GetChild(level).GetChild(0).GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f);
        }
    }

    private void UpgradeEquip(int type, int level) {
        playerData.gold -= DataManager.Instance.GetItemData(type * 10 + level).reqGold;
        equipType[type].GetChild(level).GetChild(0).gameObject.SetActive(false);
        equipType[type].GetChild(level).GetChild(1).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        equipList[type]++;
        if(type == 0) {
            Debug.Log(equipList[type]);
            playerData.rodList.Add(level);
            playerData.curRod = level;
            playerInventory.SetEquip();
        }
        DefaultSetting();
        DataManager.Instance.SavePlayerData();
        DataManager.Instance.SaveInventoryData();
    }

    private void SetGoldText() {
        goldText.text = playerData.gold.ToString() + " C";
    }

    void OnDisable()
    {
        tooltipManager.HideTooltip();
    }
}
