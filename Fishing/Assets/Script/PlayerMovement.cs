using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;

    float inputValueX;
    float inputValueZ;
    float input;
    float currentSpeedX;
    float currentSpeedZ;
    bool isGrounded = true;

    public Vector3 inputVec;

    private Rigidbody rb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        currentSpeedX = inputValueX * speed;
        currentSpeedZ = inputValueZ * speed;

        inputVec = new Vector3(inputValueX, 0, inputValueZ);
        Vector3 velocity = transform.TransformDirection(new Vector3(currentSpeedX, rb.linearVelocity.y, currentSpeedZ));
        rb.linearVelocity = velocity;
    }

    public void OnMove(InputValue value) {
        inputValueX = value.Get<Vector2>().x;
        inputValueZ = value.Get<Vector2>().y;
    }

    public void OnJump(InputValue value) {
        if(value.isPressed && isGrounded) {
            Jump();
        }
    }

    public void StartFishing() {
        rb.isKinematic = true;
    }

    public void StopFishing() {
        rb.isKinematic = false;
    }

    private void Jump() {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}
