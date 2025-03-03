using System.Collections.Generic;
using UnityEngine;

public class FishMerchant : MonoBehaviour, INPC
{
    [SerializeField] int type;

    public List<ItemData>[] itemList = new List<ItemData>[5];

    public void SetNpcType()
    {
        type = 2;
    }

    public int GetNpcType()
    {   
        return type;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetNpcType();
    }
}
