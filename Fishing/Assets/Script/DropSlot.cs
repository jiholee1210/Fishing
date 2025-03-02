using UnityEngine;
using UnityEngine.EventSystems;

public class DropSlot : MonoBehaviour, IDropHandler
{
    public int slotIndex;
    public int slotType;
    public InventoryManager inventoryManager;
    public TradeManager tradeManager;
    public ISlotHandler slotHandler;

    public void OnDrop(PointerEventData eventData) {
        DraggableItem draggedItem = eventData.pointerDrag.GetComponent<DraggableItem>();

        if(!draggedItem.canDrag) return;
        
        if(slotType == 1 ^ draggedItem.slotType == 1) {
            bool check = slotType == 1 ? inventoryManager.CheckType(draggedItem.itemIndex, slotIndex) : inventoryManager.CheckType(slotIndex, draggedItem.itemIndex);
            if(!check) return;

            if(slotType == 1) {
                inventoryManager.ExchangeEquip(draggedItem.itemIndex, slotIndex);
            }
            else {
                inventoryManager.ExchangeEquip(slotIndex, draggedItem.itemIndex);
            }
        }

        if(draggedItem.slotType == 2 && slotType != 2) {
            tradeManager.BuyEquip(draggedItem.itemIndex, slotIndex);
        }
        else if (draggedItem.slotType != 2 && slotType == 2) {
            tradeManager.SellEquip(draggedItem.itemIndex);
        }
        
        if(slotType == 0 && draggedItem.slotType == 0) {
            if(draggedItem != null && draggedItem.itemIndex != slotIndex) {
                slotHandler.SwapItem(draggedItem.itemIndex, slotIndex);
            }
        }
    
    }
}
