using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActing : MonoBehaviour
{
    [SerializeField] LayerMask fishingLayer;
    [SerializeField] float rayRange;

    private PlayerInventory playerInventory;
    private PlayerMovement playerMovement;
    private CameraRot cameraRot;

    private bool canFishing = false;
    private bool isFishing = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInventory = GetComponent<PlayerInventory>();
        playerMovement = GetComponent<PlayerMovement>();
        cameraRot = GetComponent<CameraRot>();
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

    private void StartFishing() {
        cameraRot.StartFishing();
        playerMovement.StartFishing();
        EventManager.Instance.StartFishing(playerInventory);
    }

    public void EndFishing() {
        cameraRot.StopFishing();
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
