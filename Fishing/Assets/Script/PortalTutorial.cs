using UnityEngine;

public class PortalTutorial : MonoBehaviour, IPortal, IScannable
{
    [SerializeField] Transform telPos;
    string highlight;
    public Transform GetTelPosition()
    {
        Debug.Log("위치 반환" + telPos);
        return telPos;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        highlight = "이동하기";
    }

    public string GetHighlighter() {
        return highlight;
    }
}
