using UnityEngine;

public class PortalTutorial : MonoBehaviour, IPortal
{
    Vector3 telPos;
    public Vector3 GetTelPosition()
    {
        Debug.Log("위치 반환" + telPos);
        return telPos;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        telPos = new Vector3(156, -9, -975);
    }
}
