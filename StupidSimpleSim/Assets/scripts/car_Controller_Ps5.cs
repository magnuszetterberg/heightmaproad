
using UnityEngine;

public class CarController : MonoBehaviour
{
    public float accelerationPower = 20f;
    public float reversePower = 10f;
    public float turnPower = 100f;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // Check for acceleration (R1 button)
        if (Input.GetKey(KeyCode.JoystickButton5)) // This assumes R1 is mapped to JoystickButton5
        {
            rb.AddForce(transform.forward * accelerationPower);
        }

        // Check for deceleration/reverse (L1 button)
        if (Input.GetKey(KeyCode.JoystickButton4)) // This assumes L1 is mapped to JoystickButton4
        {
            rb.AddForce(-transform.forward * reversePower);
        }

        // Steering using the Horizontal axis (typically the left stick)
        float turnInput = Input.GetAxis("Horizontal");
        rb.AddTorque(Vector3.up * turnInput * turnPower);
    }
}
