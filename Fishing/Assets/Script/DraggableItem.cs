using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform parentAfterDrag;
    private Vector3 savePos;
    private CanvasGroup canvasGroup;
    private bool isDragging = false;  // 드래그 진행중인지 저장하는 변수

    public bool canDrag = false;
    public int itemIndex;
    public int slotType; // 인벤토리 = 0, 장비창 = 1, 상점창 = 2
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update() {
        canvasGroup.interactable = canDrag;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (!canDrag) return;
        
        isDragging = true;
        parentAfterDrag = transform.parent;
        savePos = transform.localPosition;
        transform.SetParent(transform.root);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData) {
        if (!isDragging) return;
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (!isDragging) return;
        
        transform.SetParent(parentAfterDrag);
        transform.localPosition = savePos;
        canvasGroup.blocksRaycasts = true;
        isDragging = false;
    }
}
