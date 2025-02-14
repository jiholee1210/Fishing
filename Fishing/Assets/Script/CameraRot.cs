using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRot : MonoBehaviour
{
    [SerializeField] private float rotSpeed = 4f;

    private Vector2 lookInput;
    private float currentYAngle = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        float mouseX = lookInput.x * rotSpeed * Time.deltaTime;
        float mouseY = lookInput.y * rotSpeed * Time.deltaTime;
        
        currentYAngle -= mouseY;
        currentYAngle = Mathf.Clamp(currentYAngle, -50f, 30f);

        transform.Rotate(Vector3.up, mouseX);
        Camera.main.transform.localRotation = Quaternion.Euler(currentYAngle, 0, 0);
    }

    private void OnLook(InputValue value) {
        lookInput = value.Get<Vector2>();
    }
}
