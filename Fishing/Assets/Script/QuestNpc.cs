using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

public class QuestNpc : MonoBehaviour, INPC, IQuest
{
    [SerializeField] int npcID;

    private LocalizedString localizedString = new LocalizedString("DialogTable", "highlight_talk");
    private string highlight;

    void Start()
    {
        highlight = localizedString.GetLocalizedString();
    }

    public int GetNpcType()
    {
        return 3;
    }

    public string GetHighlighter()
    {
        return highlight;
    }

    public int GetNpcID()
    {
        return npcID;
    }
}
