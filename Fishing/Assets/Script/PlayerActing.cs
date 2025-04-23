using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActing : MonoBehaviour
{
    [SerializeField] LayerMask fishingLayer;
    [SerializeField] LayerMask collisionLayer;
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
    private bool isEnding = false;

    private int npcType;
    private int layer;

    private GameObject npcObject;
    private GameObject curObject;

    public PlayerData playerData;
    private List<FishData> fishList;

    private Coroutine fishingCoroutine;

    private enum UIState
    {
        None,
        Inventory,
        Quest,
        Fishing,
        Guide,
        Option,
        NPC,
        Sign,
        Skin
    }

    private enum Layer {
        None = 0,
        Npc = 7,
        Sign = 9,
        Portal = 10
    }

    private UIState currentUIState = UIState.None;
    private Layer currentLayer = Layer.None;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInventory = GetComponent<PlayerInventory>();
        playerMovement = GetComponent<PlayerMovement>();
        cameraRot = GetComponent<CameraRot>();

        playerData = DataManager.Instance.playerData;;
    }

    // Update is called once per frame
    void Update()
    {
        CheckFishingZone();
        CheckNPC();
        if(Input.GetKeyDown(KeyCode.Tab) && !isTalking) {
            if(currentUIState == UIState.Inventory) {
                CloseUI();
            }
            else if(currentUIState == UIState.None) {
                OpenUI(UIState.Inventory);
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
        if(Input.GetKeyDown(KeyCode.K) && !isTalking) {
            if(currentUIState == UIState.Skin) {
                CloseUI();
            }
            else if(currentUIState == UIState.None) {
                OpenUI(UIState.Skin);
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape) && currentUIState != UIState.Fishing && !isEnding) {
            if(currentUIState != UIState.None) {
                CloseAllWindows();
            }
            else {
                SoundManager.Instance.OpenUI();
                EventManager.Instance.OpenOptionUI();
                cameraRot.StartOtherJob();
                playerMovement.StartOtherJob();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                currentUIState = UIState.Option;
            } 
        }
    }

    public void CloseAllWindows() {
        EventManager.Instance.CloseAllWindows();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentUIState = UIState.None;
        cameraRot.StopOtherJob();
        playerMovement.StopOtherJob();
        isTalking = false;
    }
    
    public IEnumerator SetAnimator() {
        yield return new WaitForEndOfFrame();

        if(handPos.childCount > 0) {
            animator = handPos.GetChild(0).GetComponent<Animator>();
        }
    }

    public void OnAttack(InputValue value) {
        if(value.isPressed && canFishing && currentUIState == UIState.None) {
            if(playerInventory.isFishFull()) {
                Debug.Log("낚시 가방이 꽉 찼습니다.");
                EventManager.Instance.InventoryFull();
                SoundManager.Instance.ActingFailSound();
                return;
            }
            else if(playerMovement.isGrounded) {
                currentUIState = UIState.Fishing;
                StartFishing();
            } 
        }
        else if(value.isPressed && currentUIState == UIState.Fishing && !isFishing) {
            EndFishing();
            OnFishingEnd?.Invoke();
        }
    }

    public void OnInteract(InputValue value) {
        if(value.isPressed && canTalk && !isTalking) {
            curObject = npcObject;
            currentLayer = (Layer)layer;
            switch(currentLayer) {
                case Layer.Npc:
                    SoundManager.Instance.OpenUI();
                    npcType = curObject.GetComponent<INPC>().GetNpcType();
                    currentUIState = UIState.NPC;
                    EventManager.Instance.OpenNPCUI(npcType, curObject);
                    break;
                case Layer.Sign:
                    currentUIState = UIState.Sign;
                    EventManager.Instance.OpenSignUI();
                    break;
                case Layer.Portal:
                    CheckEnable(curObject);
                    return;
            }
            cameraRot.StartOtherJob();
            playerMovement.StartOtherJob();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isTalking = true;
            Debug.Log("NPC와 상호작용");
        }
    }

    public void EndTalk() {
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
        SoundManager.Instance.SwingRod();
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
        Debug.Log("현재 낚시중 : " + isFishing);
    }

    public void SetEndingState() {
        isEnding = true;
    }

    private void CheckFishingZone() {
        RaycastHit hit;
        Vector3 origin = Camera.main.transform.position + Camera.main.transform.forward * rayRange + Vector3.up * 3f;
        
        if (!Physics.Raycast(origin, Vector3.down, out hit, 10f, fishingLayer)) {
            SetHighliterState("");
            canFishing = false;
            return;
        }

        Physics.Raycast(hit.point + Vector3.up * 10f, Vector3.down, out RaycastHit groundHit, 20f);

        bool isFishingZone = hit.point.y >= groundHit.point.y - 0.05f;
        if (isFishingZone) {
            fishList = hit.collider.GetComponent<IFishingZone>().GetFishList();
            string highlight = hit.collider.GetComponent<IFishingZone>().GetHighlighter();
            SetHighliterState(highlight);
            canFishing = isFishingZone;  
        } 
        else {
            SetHighliterState("");
            canFishing = false;
            return;
        }
    }

    private void SetHighliterState(string text) {
        if(highlighter.GetComponent<TMP_Text>().text == text) return;
        highlighter.GetComponent<TMP_Text>().text = text;
    }

    private void CheckNPC() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;


        if(Physics.Raycast(ray, out hit, rayRange, collisionLayer)) {
            canTalk = true;
            layer = hit.collider.gameObject.layer;
            npcObject = hit.collider.gameObject;
            string highlight = hit.collider.GetComponent<IScannable>() != null ? hit.collider.GetComponent<IScannable>().GetHighlighter() : "보기";
            SetHighliterState(highlight);
        }
        else {
            canTalk = false;
        }
    }

    private void CheckEnable(GameObject npcObject) {
        int reqQeustID = npcObject.GetComponent<IPortal>().GetReqQuestID();
        
        if(playerData.completeQuest.Contains(reqQeustID)) {
            playerMovement.SetPos(curObject.GetComponent<IPortal>().GetTelPosition());
        }
        else {
            Debug.Log("요구 퀘스트 : " + DataManager.Instance.GetQuestData(reqQeustID).questName);
            EventManager.Instance.NotClearQuest();
            SoundManager.Instance.ActingFailSound();
        }
    }

    private void OpenUI(UIState newState)
    {
        currentUIState = newState;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        cameraRot.StartOtherJob();
        SoundManager.Instance.OpenUI();
        switch(newState)
        {
            case UIState.Inventory:
                EventManager.Instance.OpenFishInventory();
                break;
            case UIState.Guide:
                EventManager.Instance.OpenGuide();
                break;
            case UIState.Skin:
                EventManager.Instance.OpenSkin();
                break;
        }
    }

    private void CloseUI()
    {
        switch(currentUIState)
        {
            case UIState.Inventory:
                EventManager.Instance.CloseFishInventory();
                break;
            case UIState.Guide:
                EventManager.Instance.CloseGuide();
                break;
            case UIState.Skin:
                EventManager.Instance.CloseSkin();
                break;
        }
        SoundManager.Instance.OpenUI();
        currentUIState = UIState.None;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraRot.StopOtherJob();
    }

    public Vector3 GetPos() {
        return transform.position;
    }
}
