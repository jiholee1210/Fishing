using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActing : MonoBehaviour
{
    [SerializeField] LayerMask fishingLayer;
    private PlayerInventory playerInventory;

    private bool canFishing = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInventory = GetComponent<PlayerInventory>();   
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
        playerInventory.GetFish(input);
    }

    private void CheckFishingZone() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, Mathf.Infinity, fishingLayer)) {
            canFishing = true;
        }
        else {
            canFishing = false;
        }
    }
}
