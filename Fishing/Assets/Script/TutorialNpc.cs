using System.Collections.Generic;
using UnityEngine;

public class TutorialNpc : MonoBehaviour, INPC, IQuest
{
    [SerializeField] int npcType;
    private string line;
    private string highlight;
    private int npcID;


    public int GetNpcType()
    {
        return npcType;
    }

    public void SetNpcType()
    {
        npcType = 3;
    }

    public string GetLine() {
        return line;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        line = "외지인은 오랜만이구만. 무슨 일인가?";
        highlight = "대화하기";
        SetNpcType();
        SetNpcID();
    }

    public string GetHighlighter()
    {
        return highlight;
    }

    public void SetNpcID()
    {
        npcID = 0;
    }

    public int GetNpcID()
    {
        return npcID;
    }
}
