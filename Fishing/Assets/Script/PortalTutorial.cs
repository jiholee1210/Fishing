using UnityEngine;

public class PortalTutorial : MonoBehaviour, IPortal, IScannable
{
    [SerializeField] Transform telPos;
    string highlight;
    public Vector3 GetTelPosition()
    {
        Debug.Log("위치 반환" + telPos);
        return telPos.position;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        highlight = "이동하기";
    }

    public string GetHighlighter() {
        return highlight;
    }

    public int GetReqQuestID()
    {
       return 2;
    }
}
