using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    //플레이어 물고기 리스트 + 착용 중 장비 + 소지중인 장비
    [SerializeField] Image[] slots;

    private List<ItemData> itemList;

    void Start()
    {
        int listSize = 54;
        itemList = new List<ItemData>(new ItemData[listSize]);
    }

    public void AddFishToSlot(int id) {

    }

    public void AddEquipToSlot(int id) {
        ItemData itemData = DataManager.Instance.GetItemData(id);
        for(int i = 0; i < itemList.Count; i++) {
            if(itemList[i] == null) {
                itemList[i] = itemData;
                slots[i].sprite = itemData.itemImage;
                Debug.Log("아이템 이벤토리에 추가");
                break;
            }
        }

    }

    public void SetEquip() {

    }
}
