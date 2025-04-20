using UnityEngine;
using UnityEngine.Localization;

public class NPC : MonoBehaviour, INPC
{
    [SerializeField] private int npcType;

    private LocalizedString localizedString = new LocalizedString("DialogTable", "highlight_talk");
    private string highlight;

    void Start()
    {
        highlight = localizedString.GetLocalizedString();
    }

    public string GetHighlighter()
    {
        return highlight;
    }

    public int GetNpcType()
    {
        return npcType;
    }
}
