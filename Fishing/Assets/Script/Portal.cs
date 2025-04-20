using UnityEngine;
using UnityEngine.Localization;

public class Portal : MonoBehaviour, IPortal, IScannable
{
    [SerializeField] Transform telPos;
    [SerializeField] int reqQeustID;

    private LocalizedString localizedString = new LocalizedString("DialogTable", "highlight_tel");
    private string highlight;

    void Start()
    {
        highlight = localizedString.GetLocalizedString();
    }

    public Vector3 GetTelPosition()
    {
        Debug.Log("위치 반환" + telPos);
        return telPos.position;
    }

    public string GetHighlighter() {
        return highlight;
    }
    // 초기 언어 세팅 한번만 하도록 구현

    public int GetReqQuestID()
    {
       return reqQeustID;
    }
}
