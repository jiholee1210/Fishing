using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TooltipManager tooltipManager;
    [SerializeField] public int itemID;


    public void OnPointerEnter(PointerEventData eventData)
    {
        Vector3 newPos = transform.position + new Vector3(280f, 0, 0);
        tooltipManager.ShowTooltip(itemID, newPos);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipManager.HideTooltip();
    }
}
