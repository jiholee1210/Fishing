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
    private bool isEquipInven = false;
    private bool isFishInven = false;
    private int curNpcType;
    private GameObject curNpcObject;

    public PlayerData playerData;
    private List<FishData> fishList;
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
        if(Input.GetKeyDown(KeyCode.Tab) && !isTalking && !isFishInven) {
            if(!inventoryOpen) {
                EventManager.Instance.OpenInventory();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                cameraRot.StartOtherJob();
                inventoryOpen = true;
                isEquipInven = true;
            }
            else {
                EventManager.Instance.CloseInventory();
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                cameraRot.StopOtherJob();
                inventoryOpen = false;
                isEquipInven = false;
            }
        }

        if(Input.GetKeyDown(KeyCode.I) && !isTalking && !isEquipInven) {
            if(!inventoryOpen) {
                EventManager.Instance.OpenFishInventory();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                cameraRot.StartOtherJob();
                inventoryOpen = true;
                isFishInven = true;
            }
            else {
                EventManager.Instance.CloseFishInventory();
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                cameraRot.StopOtherJob();
                inventoryOpen = false;
                isFishInven = false;
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape)) {
            EventManager.Instance.CloseAllWindows();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            cameraRot.StopOtherJob();
            playerMovement.StopOtherJob();
            inventoryOpen = false;
            isEquipInven = false;
            isFishInven = false;
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
            EventManager.Instance.OpenNPCUI(curNpcType, curNpcObject);
            cameraRot.StartOtherJob();
            playerMovement.StartOtherJob();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isTalking = true;
            Debug.Log("NPC와 상호작용");
        }
    }

    public void EndTalk() {
        EventManager.Instance.CloseNpcUI(curNpcType);
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
            curNpcType = hit.collider.GetComponent<INPC>().GetNpcType();
            curNpcObject = hit.collider.gameObject;
            Debug.DrawLine(ray.origin, hit.point, Color.green);
        }
        else {
            canTalk = false;
            curNpcType = 0;
        }
    }
}
