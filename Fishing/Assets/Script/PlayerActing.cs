using System.Collections;
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
    private int curNpcType;

    public PlayerData playerData;
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
        if(Input.GetKeyDown(KeyCode.Tab)) {
            if(!inventoryOpen) {
                EventManager.Instance.OpenInventory();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                cameraRot.StartOtherJob();
                inventoryOpen = true;
            }
            else {
                EventManager.Instance.CloseInventory();
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                cameraRot.StopOtherJob();
                inventoryOpen = false;
            }
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
            if(value.isPressed && canFishing && !isFishing) {
                isFishing = true;
                StartFishing();
            }
        }
    }

    public void OnInteract(InputValue value) {
        if(value.isPressed && canTalk) {
            EventManager.Instance.OpenNPCUI(curNpcType);
            cameraRot.StartOtherJob();
            playerMovement.StartOtherJob();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Log("NPC와 상호작용");
        }
    }

    public void EndTalk() {
        EventManager.Instance.CloseNpcUI(curNpcType);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraRot.StopOtherJob();
        playerMovement.StopOtherJob();
    }

    IEnumerator PlayFishingAnimation() {
        animator.Play("FishingSwing");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
    }

    IEnumerator FishingSequence() {
        yield return StartCoroutine(PlayFishingAnimation());
        EventManager.Instance.StartFishing(playerData, playerInventory);
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

        if(Physics.Raycast(ray, out hit, rayRange, fishingLayer)) {
            float terrainHitPoint = Terrain.activeTerrain.SampleHeight(hit.point);
            canFishing = hit.point.y >= terrainHitPoint ? true : false;

            Debug.DrawLine(ray.origin, hit.point, Color.green);
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
            Debug.DrawLine(ray.origin, hit.point, Color.green);
        }
        else {
            canTalk = false;
            curNpcType = 0;
        }
    }
}
