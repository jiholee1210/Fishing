using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActing : MonoBehaviour
{
    [SerializeField] LayerMask fishingLayer;
    [SerializeField] LayerMask npcLayer;
    [SerializeField] float rayRange;
    [SerializeField] Transform handPos;
    [SerializeField] GameObject highlighter;

    public event Action OnFishingEnd;

    public PlayerInventory playerInventory;
    private PlayerMovement playerMovement;
    private CameraRot cameraRot;
    private Animator animator;

    private bool canFishing = false;
    private bool canTalk = false;
    private bool isTalking = false;
    private bool isFishing = false;
    private bool wasFishingZone = false;

    private int npcType;
    private int curType;
    

    private GameObject npcObject;
    private GameObject curNpcObject;

    public PlayerData playerData;
    private List<FishData> fishList;

    private Coroutine fishingCoroutine;

    private enum UIState
    {
        None,
        Equipment,
        FishInventory,
        Quest,
        Fishing,
        Guide
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

        if(Input.GetKeyDown(KeyCode.G) && !isTalking) {
            if(currentUIState == UIState.Guide) {
                CloseUI();
            }
            else if(currentUIState == UIState.None) {
                OpenUI(UIState.Guide);
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape)) {
            EventManager.Instance.CloseAllWindows();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            currentUIState = UIState.None;
            cameraRot.StopOtherJob();
            playerMovement.StopOtherJob();
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
        if(playerInventory.HaveRod() && playerInventory.HaveBait()) {
            if(value.isPressed && canFishing && currentUIState == UIState.None) {
                if(playerInventory.isFishFull()) {
                    Debug.Log("낚시 가방이 꽉 찼습니다.");
                    return;
                }
                currentUIState = UIState.Fishing;
                StartFishing();
            }
            else if(value.isPressed && currentUIState == UIState.Fishing && !isFishing) {
                EndFishing();
                OnFishingEnd?.Invoke();
            }
        }
    }

    public void OnInteract(InputValue value) {
        if(value.isPressed && canTalk && !isTalking) {
            curType = npcType;
            curNpcObject = npcObject;
            Debug.Log(curNpcObject);
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
        EventManager.Instance.StartFishing(fishList);
    }

    private void StartFishing() {
        cameraRot.StartOtherJob();
        playerMovement.StartOtherJob();
        fishingCoroutine = StartCoroutine(FishingSequence());
    }

    public void EndFishing() {
        cameraRot.StopOtherJob();
        if(fishingCoroutine != null) {
            StopCoroutine(fishingCoroutine);
            fishingCoroutine = null;
        }
        animator.Play("FishingSwingBack");
        playerMovement.StopOtherJob();
        currentUIState = UIState.None;
    }

    public void SetStartFishing() {
        isFishing = !isFishing;
    }

    private void CheckFishingZone() {
        RaycastHit hit;
        Vector3 origin = Camera.main.transform.position + Camera.main.transform.forward * rayRange + Vector3.up * 3f;
        
        // 1차 레이: 정면 아래 방향, 낚시 구역 감지
        if (!Physics.Raycast(origin, Vector3.down, out hit, 6f, fishingLayer)) {
            SetFishingState(false, "");
            return;
        }

        // 2차 레이: 감지된 지점에서 아래 방향 (수면 감지)
        Physics.Raycast(hit.point + Vector3.up * 10f, Vector3.down, out RaycastHit groundHit, 20f);

        // 높이 비교 (오차 범위 적용)
        bool isFishingZone = hit.point.y >= groundHit.point.y - 0.05f;
        if (isFishingZone) {
            fishList = hit.collider.GetComponent<IFishingZone>().GetFishList();
        } 
        else {
            SetFishingState(false, "");
            return;
        }

        SetFishingState(isFishingZone, "낚시하기");
    }

    private void SetFishingState(bool state, string text) {
        if (wasFishingZone == state) return;

        wasFishingZone = state;
        canFishing = state;
        highlighter.GetComponent<TMP_Text>().text = text;
    }

    private void CheckNPC() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Debug.DrawLine(ray.origin, ray.origin + ray.direction * rayRange, Color.red);

        if(Physics.Raycast(ray, out hit, rayRange, npcLayer)) {
            canTalk = true;
            npcType = hit.collider.GetComponent<INPC>().GetNpcType();
            npcObject = hit.collider.gameObject;
            Debug.DrawLine(ray.origin, hit.point, Color.green);
        }
        else {
            canTalk = false;
            npcType = 0;
            npcObject = null;
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
            case UIState.Guide:
                EventManager.Instance.OpenGuide();
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
            case UIState.Guide:
                EventManager.Instance.CloseGuide();
                break;
        }

        currentUIState = UIState.None;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraRot.StopOtherJob();
    }
}
