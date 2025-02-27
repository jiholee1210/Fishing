using UnityEngine;
using UnityEngine.EventSystems;

public class DropSlot : MonoBehaviour, IDropHandler
{
    public int slotIndex;
    public InventoryManager inventoryManager;

    public void OnDrop(PointerEventData eventData) {
        DraggableItem draggedItem = eventData.pointerDrag.GetComponent<DraggableItem>();

        if(draggedItem != null && draggedItem.itemIndex != slotIndex) {
            inventoryManager.SwapItem(draggedItem.itemIndex, slotIndex);
        }
    }
}
