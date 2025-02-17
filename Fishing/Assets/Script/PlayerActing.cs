using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActing : MonoBehaviour
{
    [SerializeField] LayerMask fishingLayer;
    [SerializeField] float rayRange;

    private PlayerInventory playerInventory;
    private CameraRot cameraRot;
    private Rigidbody rb;

    private bool canFishing = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInventory = GetComponent<PlayerInventory>();
        cameraRot = GetComponent<CameraRot>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckFishingZone();
    }

    public void OnAttack(InputValue value) {
        if(value.isPressed && canFishing) {
            Fishing(1);
        }
    }

    private void Fishing(int input) {
        cameraRot.StartFishing();
        rb.isKinematic = true;
        UIManager.Instance.OpenFishingUI(playerInventory);
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
