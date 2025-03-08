using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActing : MonoBehaviour
{
    [SerializeField] LayerMask fishingLayer;
    [SerializeField] LayerMask npcLayer;
    [SerializeField] float rayRange;
    [SerializeField] Transform handPos;

    private PlayerInventory playerInventory;
    private PlayerMovement playerMovement;
    private CameraRot cameraRot;
    private Animator animator;

    private bool canFishing = false;
    private bool isFishing = false;
    private bool inventoryOpen = false;
    private bool canTalk = false;
    private bool isTalking = false;

    private int npcType;
    private int curType;

    private GameObject curNpcObject;

    public PlayerData playerData;
    private List<FishData> fishList;

    private enum UIState
    {
        None,
        Equipment,
        FishInventory,
        Quest
    }

    private UIState currentUIState = UIState.None;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInventory = GetComponent<PlayerInventory>();
        playerMovement = GetComponent<PlayerMovement>();
        cameraRot = GetComponent<CameraRot>();

        playerData = DataManager.Instance.playerData;
    }

    // Update is called once per frame
    void Update()
    {
        CheckFishingZone();
        CheckNPC();
        if(Input.GetKeyDown(KeyCode.Tab) && !isTalking) {
            if(currentUIState == UIState.Equipment) {
                CloseUI();
            }
            else if(currentUIState == UIState.None) {
                OpenUI(UIState.Equipment);
            }
        }

        if(Input.GetKeyDown(KeyCode.I) && !isTalking) {
            if(currentUIState == UIState.FishInventory) {
                CloseUI();
            }
            else if(currentUIState == UIState.None) {
                OpenUI(UIState.FishInventory);
            }
        }

        if(Input.GetKeyDown(KeyCode.Q) && !isTalking) {
            if(currentUIState == UIState.Quest) {
                CloseUI();
            }
            else if(currentUIState == UIState.None) {
                OpenUI(UIState.Quest);
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape)) {
            EventManager.Instance.CloseAllWindows();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            currentUIState = UIState.None;
            cameraRot.StopOtherJob();
            playerMovement.StopOtherJob();
            inventoryOpen = false;
            isTalking = false;
        }
    }

    public IEnumerator SetAnimator() {
        yield return new WaitForEndOfFrame();

        if(handPos.childCount > 0) {
            Debug.Log("애니메이터 설정됨");
            animator = handPos.GetChild(0).GetComponent<Animator>();
        }
    }

    public void OnAttack(InputValue value) {
        if(playerInventory.haveRod()) {
            if(value.isPressed && canFishing && !isFishing && !inventoryOpen) {
                if(playerInventory.isFishFull()) {
                    Debug.Log("낚시 가방이 꽉 찼습니다.");
                    return;
                }
                if(playerInventory.NotEquip()) {
                    Debug.Log("장비를 모두 장착하지 않았습니다.");
                    return;
                }
                isFishing = true;
                StartFishing();
            }
        }
    }

    public void OnInteract(InputValue value) {
        if(value.isPressed && canTalk && !isTalking) {
            curType = npcType;
            EventManager.Instance.OpenNPCUI(curType, curNpcObject);
            cameraRot.StartOtherJob();
            playerMovement.StartOtherJob();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isTalking = true;
            Debug.Log("NPC와 상호작용");
        }
    }

    public void EndTalk() {
        EventManager.Instance.CloseNpcUI(curType);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraRot.StopOtherJob();
        playerMovement.StopOtherJob();
        isTalking = false;
    }

    IEnumerator PlayFishingAnimation() {
        animator.Play("FishingSwing");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
    }

    IEnumerator FishingSequence() {
        yield return StartCoroutine(PlayFishingAnimation());
        EventManager.Instance.StartFishing(playerInventory, fishList);
    }

    private void StartFishing() {
        cameraRot.StartOtherJob();
        playerMovement.StartOtherJob();
        StartCoroutine(FishingSequence());
    }

    public void EndFishing() {
        cameraRot.StopOtherJob();
        animator.Play("FishingSwingBack");
        playerMovement.StopOtherJob();
        isFishing = false;
    }

    private void CheckFishingZone() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Debug.DrawLine(ray.origin, ray.origin + ray.direction * rayRange, Color.red);

        if(Physics.Raycast(ray, out hit, 6f, fishingLayer)) {
            // 수면 위치를 체크하기 위한 두 번째 레이캐스트
            Ray downRay = new Ray(hit.point + Vector3.up * 10f, Vector3.down);
            RaycastHit groundHit;
            
            if(Physics.Raycast(downRay, out groundHit, 20f)) {
                canFishing = hit.point.y >= groundHit.point.y;
                fishList = hit.collider.GetComponent<IFishingZone>().GetFishList();
            }

            Debug.DrawLine(ray.origin, hit.point, Color.green);
            Debug.DrawLine(downRay.origin, groundHit.point, Color.red);
        }
        else {
            canFishing = false;
        }
    }

    private void CheckNPC() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Debug.DrawLine(ray.origin, ray.origin + ray.direction * rayRange, Color.red);

        if(Physics.Raycast(ray, out hit, rayRange, npcLayer)) {
            canTalk = true;
            npcType = hit.collider.GetComponent<INPC>().GetNpcType();
            curNpcObject = hit.collider.gameObject;
            Debug.DrawLine(ray.origin, hit.point, Color.green);
        }
        else {
            canTalk = false;
            npcType = 0;
        }
    }

    private void OpenUI(UIState newState)
    {
        currentUIState = newState;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        cameraRot.StartOtherJob();

        switch(newState)
        {
            case UIState.Equipment:
                EventManager.Instance.OpenInventory();
                break;
            case UIState.FishInventory:
                EventManager.Instance.OpenFishInventory();
                break;
            case UIState.Quest:
                EventManager.Instance.OpenQuest();
                break;
        }
    }

    private void CloseUI()
    {
        switch(currentUIState)
        {
            case UIState.Equipment:
                EventManager.Instance.CloseInventory();
                break;
            case UIState.FishInventory:
                EventManager.Instance.CloseFishInventory();
                break;
            case UIState.Quest:
                EventManager.Instance.CloseQuest();
                break;
        }

        currentUIState = UIState.None;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraRot.StopOtherJob();
    }
}
