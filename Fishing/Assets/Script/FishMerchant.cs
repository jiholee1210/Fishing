using System.Collections.Generic;
using UnityEngine;

public class FishMerchant : MonoBehaviour, INPC
{
    [SerializeField] int type;

    string line;

    public void SetNpcType()
    {
        type = 2;
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
        line = "어느 물고기든 가져오기만 하면 환영일세.";
        SetNpcType();
    }

    public string GetHighlighter()
    {
        return "대화하기";
    }
}
