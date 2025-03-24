using UnityEngine;
using UnityEngine.EventSystems;

public class DropSlot : MonoBehaviour, IDropHandler
{
    public int slotIndex;
    public int slotType;
    public ISlotHandler slotHandler;

    public void OnDrop(PointerEventData eventData) {
        DraggableItem draggedItem = eventData.pointerDrag.GetComponent<DraggableItem>();

        if(!draggedItem.canDrag) return;
        
        if(slotType == 0 && draggedItem.slotType == 0) {
            if(draggedItem != null && draggedItem.itemIndex != slotIndex) {
                slotHandler.SwapItem(draggedItem.itemIndex, slotIndex);
            }
        }
    
    }
}
