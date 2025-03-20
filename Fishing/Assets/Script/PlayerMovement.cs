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

    private Vector3 slidingVelocity;
    private bool isSliding = false;

    private CharacterController characterController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        characterController.slopeLimit = 45f;

        SetPos(DataManager.Instance.playerData.pos);
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
        ApplySliding();
    }

    public void OnMove(InputValue value) {
        if(!cantMove) {
            inputValueX = value.Get<Vector2>().x;
            inputValueZ = value.Get<Vector2>().y;
        }
    }

    public void OnJump(InputValue value) {
        if(value.isPressed && isGrounded && !cantMove && !isSliding) {
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

    public void SetPos(Vector3 pos) {
        characterController.enabled = false;
        transform.position = pos;
        characterController.enabled = true;
    }

    private void ApplySliding() {
        if(!isGrounded) return;
        
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, 10f)) {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            if(Vector3.Angle(hit.normal, Vector3.up) > characterController.slopeLimit) {
                
                Debug.Log("경사면 감지");
                isSliding = true;
                Vector3 slopeDir = Vector3.ProjectOnPlane(Vector3.down, hit.normal).normalized;
                slidingVelocity += slopeDir * gravity * Time.deltaTime;
                characterController.Move(slidingVelocity * Time.deltaTime);
            }
            else {
                isSliding = false;
                slidingVelocity = Vector3.zero;
            }
        }
        
    }
}
