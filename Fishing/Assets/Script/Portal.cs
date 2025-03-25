using UnityEngine;

public class Portal : MonoBehaviour, IPortal, IScannable
{
    [SerializeField] Transform telPos;
    [SerializeField] Material skybox;
    [SerializeField] string highlight;
    [SerializeField] int reqQeustID;
    
    public Vector3 GetTelPosition()
    {
        Debug.Log("위치 반환" + telPos);
        RenderSettings.skybox = skybox;
        DynamicGI.UpdateEnvironment();
        return telPos.position;
    }

    public string GetHighlighter() {
        return highlight;
    }

    public int GetReqQuestID()
    {
       return reqQeustID;
    }
}
