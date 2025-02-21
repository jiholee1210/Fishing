using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActing : MonoBehaviour
{
    [SerializeField] LayerMask fishingLayer;
    [SerializeField] float rayRange;

    private PlayerInventory playerInventory;
    private PlayerMovement playerMovement;
    private CameraRot cameraRot;
    private Animator animator;

    private bool canFishing = false;
    private bool isFishing = false;

    public PlayerData playerData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Animator>();
        playerInventory = GetComponent<PlayerInventory>();
        playerMovement = GetComponent<PlayerMovement>();
        cameraRot = GetComponent<CameraRot>();

        playerData = DataManager.Instance.playerData;
    }

    // Update is called once per frame
    void Update()
    {
        CheckFishingZone();
    }

    public void OnAttack(InputValue value) {
        if(value.isPressed && canFishing && !isFishing) {
            isFishing = true;
            StartFishing();
        }
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
        cameraRot.StartFishing();
        playerMovement.StartFishing();
        StartCoroutine(FishingSequence());
    }

    public void EndFishing() {
        cameraRot.StopFishing();
        animator.Play("FishingSwingBack");
        playerMovement.StopFishing();
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
}
