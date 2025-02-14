using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;

    float inputValueX;
    float inputValueZ;
    float input;
    float currentSpeedX;
    float currentSpeedZ;

    public Vector3 inputVec;

    private Rigidbody rigidbody;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
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
        Vector3 velocity = transform.TransformDirection(new Vector3(currentSpeedX, rigidbody.linearVelocity.y, currentSpeedZ));
        rigidbody.linearVelocity = velocity;
    }

    public void OnMove(InputValue value) {
        inputValueX = value.Get<Vector2>().x;
        inputValueZ = value.Get<Vector2>().y;
    }
}
