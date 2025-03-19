using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravity;

    float inputValueX;
    float inputValueZ;
    float currentSpeedX;
    float currentSpeedZ;
    Vector3 velocity;
    bool isGrounded;
    bool cantMove = false;

    private CharacterController characterController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        characterController.slopeLimit = 45f;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = characterController.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // 살짝 바닥에 붙게 설정 (중력 버그 방지)
        }
        if(!cantMove) {
            currentSpeedX = inputValueX * speed;
            currentSpeedZ = inputValueZ * speed;

            Vector3 moveDirection = transform.TransformDirection(new Vector3(currentSpeedX, 0, currentSpeedZ));
            characterController.Move(moveDirection * Time.deltaTime);
        }

        velocity.y -= gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    void FixedUpdate()
    {
        
    }

    public void OnMove(InputValue value) {
        if(!cantMove) {
            inputValueX = value.Get<Vector2>().x;
            inputValueZ = value.Get<Vector2>().y;
        }
    }

    public void OnJump(InputValue value) {
        if(value.isPressed && isGrounded && !cantMove) {
            velocity.y = jumpForce;
        }
    }

    public void StartOtherJob() {
        cantMove = true;
        inputValueX = 0f;
        inputValueZ = 0f;
    }

    public void StopOtherJob() {
        cantMove = false;
    }

    public void Teleport(Transform pos) {
        characterController.enabled = false;
        transform.position = pos.position;
        characterController.enabled = true;
    }
}
