using System.Collections.Generic;
using UnityEngine;

public class Merchant : MonoBehaviour, INPC, IMerchant
{
    [SerializeField] int type;
    string line;

    public List<ItemData>[] itemList = new List<ItemData>[5];

    public void SetNpcType()
    {
        type = 1;
    }

    public int GetNpcType()
    {   
        return type;
    }

    public string GetLine() {
        return line;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetNpcType();
        line = "좋은 물건을 싸게 팔고 있으니 맘껏 둘러보라고.";
        for (int i = 0; i < itemList.Length; i++) {
            itemList[i] = new List<ItemData>();
            Debug.Log(i + " 번째 리스트 초기화");
        }

        itemList[0].Add(DataManager.Instance.GetItemData(1));
        itemList[0].Add(DataManager.Instance.GetItemData(2));
    }

    public List<ItemData> GetItemList(int category) {
        Debug.Log(category);
        return itemList[category];
    }

    public string GetHighlighter()
    {
        return "대화하기";
    }
}
