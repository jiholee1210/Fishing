using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform parentAfterDrag;
    private Vector3 savePos;
    private CanvasGroup canvasGroup;

    public int itemIndex;
    public int slotType; // 인벤토리 = 0, 장비창 = 1, 상점창 = 2
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData) {
        parentAfterDrag = transform.parent;
        savePos = transform.localPosition;
        transform.SetParent(transform.root);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData) {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData) {
        transform.SetParent(parentAfterDrag);
        transform.localPosition = savePos;
        canvasGroup.blocksRaycasts = true;
    }
}
