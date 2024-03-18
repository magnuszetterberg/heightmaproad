
using UnityEngine;
using Cinemachine;

public class CameraFOVAdjuster : MonoBehaviour
{
    public CinemachineFreeLook cinemachineFreeLookCamera; // Reference to your Cinemachine FreeLook Camera
    public PrometeoCarController carController; // Reference to your car controller script
    public float minFOV = 60f; // Minimum FOV
    public float maxFOV = 90f; // Maximum FOV
    public float startChangeSpeed = 30f; // The speed at which the FOV starts to change
    public float maxSpeed = 100f; // The speed at which the FOV will reach its maximum

    private float originalFOV; // To store the original FOV

    void Start()
    {
        // Store the original FOV value
        originalFOV = cinemachineFreeLookCamera.m_Lens.FieldOfView;
    }

    void Update()
    {
        if (carController != null) // Check if the carController is assigned
        {
            // Get the current speed of the car from your car controller
            float currentSpeed = carController.carSpeed;

            // Check if the current speed is greater than the startChangeSpeed
            if(currentSpeed > startChangeSpeed)
            {
                // Calculate the adjusted speed for FOV change
                float adjustedSpeed = currentSpeed - startChangeSpeed;
                // Ensure adjustedSpeed does not exceed maxSpeed for FOV calculation
                adjustedSpeed = Mathf.Min(adjustedSpeed, maxSpeed - startChangeSpeed);
                
                // Map the adjusted speed to the FOV range
                float targetFOV = Mathf.Lerp(originalFOV, maxFOV, adjustedSpeed / (maxSpeed - startChangeSpeed));
                
                // Apply the target FOV to the Cinemachine FreeLook camera
                cinemachineFreeLookCamera.m_Lens.FieldOfView = targetFOV;
            }
        }
        else
        {
            Debug.LogError("CarController reference not set on CameraFOVAdjuster script.");
        }
    }
}
