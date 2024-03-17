
using UnityEngine;

public class CarResetHandler : MonoBehaviour
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Rigidbody carRigidbody;

    void Start()
    {
        // Store the original position and rotation
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        // Get the Rigidbody component from the car
        carRigidbody = GetComponent<Rigidbody>();
    }

    // Method to reset car's position and rotation to the original values
    public void ResetCarPosition()
    {
        if (carRigidbody != null)
        {
            // Stop the car's movement and rotation
            carRigidbody.velocity = Vector3.zero;
            carRigidbody.angularVelocity = Vector3.zero;
        }

        // Reset the car's position and rotation
        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }
}
