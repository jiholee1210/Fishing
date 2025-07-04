using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravity;
    [SerializeField] Transform handPos;
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private WipeController wipeController;


    float inputValueX;
    float inputValueZ;
    float currentSpeedX;
    float currentSpeedZ;
    Vector3 velocity;
    public bool isGrounded;
    bool cantMove = false;

    private Vector3 slidingVelocity;
    private bool isSliding = false;
    private bool isWalking = false;

    private float time = 0f;

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

            if((inputValueX != 0 || inputValueZ  != 0) && isGrounded && velocity.y <= 0) {
                WalkingAnimation();
                PlayWalkingSound();
            }
        }

        velocity.y -= gravity * Time.deltaTime;
        if(characterController.enabled) {
            characterController.Move(velocity * Time.deltaTime);
        }
        ApplySliding();
    }

    public void OnMove(InputValue value) {
        if(!cantMove) {
            Vector2 input = value.Get<Vector2>();
            if(input != Vector2.zero) {
                inputValueX = value.Get<Vector2>().x;
                inputValueZ = value.Get<Vector2>().y;
            }
            else {
                inputValueX = 0f;
                inputValueZ = 0f;
                handPos.GetChild(0).localPosition = new Vector3(0f, 0f, 0f);
                handPos.GetChild(0).localEulerAngles = new Vector3(0f, 0f, 0f);
                time = 0f;
                StopWalkingSound();
            }
        }
    }

    public void OnJump(InputValue value) {
        if(value.isPressed && isGrounded && !cantMove && !isSliding) {
            velocity.y = jumpForce;
            handPos.GetChild(0).localPosition = new Vector3(0f, 0f, 0f);
            handPos.GetChild(0).localEulerAngles = new Vector3(0f, 0f, 0f);
            time = 0f;
            StopWalkingSound();
        }
    }

    public void StartOtherJob() {
        StopWalkingSound();
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

    public IEnumerator FallIntoWater() {
        soundManager.FallWater();
        characterController.enabled = false;
        cantMove = true;
        yield return StartCoroutine(wipeController.CircleIn());
        transform.position = new Vector3(1275.5f, -75.2f, 1921.3f);
        yield return StartCoroutine(wipeController.CircleOut());
        inputValueX = 0f;
        inputValueZ = 0f;
        cantMove = false;
        characterController.enabled = true;
    }

    public void LockControl() {
        characterController.enabled = false;
    }

    public void UnlockControl() {
        characterController.enabled = true;
    }

    private void ApplySliding() {
        if(!isGrounded) return;
        
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, 10f)) {
            if(Vector3.Angle(hit.normal, Vector3.up) > characterController.slopeLimit) {
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

    private void WalkingAnimation() {
        if(handPos.childCount > 0) {
            time += Time.deltaTime;

            float yOffset = Mathf.Cos(time * 2 * Mathf.PI) * 0.1f;
            float zRotation = -Mathf.Cos(time * 2 * Mathf.PI) * 7.5f + 7.5f;

            handPos.GetChild(0).localPosition = new Vector3(0f, yOffset, 0f);
            handPos.GetChild(0).localEulerAngles = new Vector3(0f, 0f, zRotation);
        }
    }

    private void PlayWalkingSound() {
        if(!isWalking) {
            soundManager.PlayWalkingSound();
            isWalking = true;
        }
    }

    private void StopWalkingSound() {
        isWalking = false;
        soundManager.StopWalkingSound();
    }
}
