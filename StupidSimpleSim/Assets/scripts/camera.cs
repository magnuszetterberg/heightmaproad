
using UnityEngine;

public class FlyCamera : MonoBehaviour
{
    public float lookSpeed = 3.0f;
    public float moveSpeed = 5.0f;
    public float boostMultiplier = 2.0f; // Speed multiplier when shift is held down

    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    void Start()
    {
        // Initialize with current rotation
        Vector3 currentRotation = transform.eulerAngles;
        rotationX = currentRotation.y;
        rotationY = currentRotation.x;
    }

    void Update()
    {
        // Right mouse button pressed
        if (Input.GetMouseButton(1))
        {
            // Mouse look
            rotationX += Input.GetAxis("Mouse X") * lookSpeed;
            rotationY -= Input.GetAxis("Mouse Y") * lookSpeed; // Subtract to invert Y axis
            rotationY = Mathf.Clamp(rotationY, -90, 90); // Clamp vertical rotation

            transform.localEulerAngles = new Vector3(rotationY, rotationX, 0.0f);
        }

        // Movement
        float currentMoveSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentMoveSpeed *= boostMultiplier; // Boost the move speed
        }

        transform.position += transform.forward * currentMoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
        transform.position += transform.right * currentMoveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;

        // Optional: Add vertical movement if you want to allow flying up and down
        if (Input.GetKey(KeyCode.E))
        {
            transform.position += transform.up * currentMoveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.position -= transform.up * currentMoveSpeed * Time.deltaTime;
        }
    }
}
