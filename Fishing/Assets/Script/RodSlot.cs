using UnityEngine;
using UnityEngine.EventSystems;

public class RodSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TooltipManager tooltipManager;
    
    public int slotID;
    public void OnPointerEnter(PointerEventData eventData)
    {
        Vector3 newPos = transform.position + new Vector3(410f, 45f, 0f);
        tooltipManager.ShowRod(slotID, newPos);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipManager.HideRod(slotID);
    }
}
