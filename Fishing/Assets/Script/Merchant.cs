using System.Collections.Generic;
using UnityEngine;

public class Merchant : MonoBehaviour, INPC, IMerchant
{
    [SerializeField] int type;
    string line;

    public List<ItemData> itemList = new();

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

        itemList.Add(DataManager.Instance.GetItemData(0));
        itemList.Add(DataManager.Instance.GetItemData(1));
        itemList.Add(DataManager.Instance.GetItemData(10));
        itemList.Add(DataManager.Instance.GetItemData(20));
        itemList.Add(DataManager.Instance.GetItemData(30));
    }

    public List<ItemData> GetItemList() {
        return itemList;
    }

    public string GetHighlighter()
    {
        return "대화하기";
    }
}
