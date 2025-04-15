using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRot : MonoBehaviour
{
    [SerializeField] public float rotSpeed;

    private Vector2 lookInput;
    private float currentYAngle = 0f;

    private bool cantMove = false;
    private bool isInventoryOpen = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(cantMove) {
            lookInput = Vector2.zero;
        }

        if(isInventoryOpen) {
            lookInput = Vector2.zero;
        }

        else {
            float mouseX = lookInput.x * rotSpeed * Time.deltaTime;
            float mouseY = lookInput.y * rotSpeed * Time.deltaTime;
            
            currentYAngle -= mouseY;
            currentYAngle = Mathf.Clamp(currentYAngle, -90f, 90f);

            transform.Rotate(Vector3.up, mouseX);
            Camera.main.transform.localRotation = Quaternion.Euler(currentYAngle, 0, 0);
        }
    }

    public void StartOtherJob() {
        cantMove = true;
    }

    public void StopOtherJob() {
        cantMove = false;
    }

    private void OnLook(InputValue value) {
        lookInput = value.Get<Vector2>();
    }
}
