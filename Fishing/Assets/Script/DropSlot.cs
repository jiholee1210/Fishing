using UnityEngine;
using UnityEngine.EventSystems;

public class DropSlot : MonoBehaviour, IDropHandler
{
    public int slotIndex;
    public bool isEquipSlot;
    public InventoryManager inventoryManager;

    public void OnDrop(PointerEventData eventData) {
        DraggableItem draggedItem = eventData.pointerDrag.GetComponent<DraggableItem>();

        if(draggedItem != null && draggedItem.itemIndex != slotIndex) {
            if(isEquipSlot ^ draggedItem.isEquipSlot) {
                bool check = isEquipSlot ? inventoryManager.CheckType(draggedItem.itemIndex, slotIndex) : inventoryManager.CheckType(slotIndex, draggedItem.itemIndex);
                if(!check) return;

                if(isEquipSlot) {
                    inventoryManager.ExchangeEquip(draggedItem.itemIndex, slotIndex);
                    return;
                }
                
            }
            inventoryManager.SwapItem(draggedItem.itemIndex, slotIndex);
        }
    }
}
