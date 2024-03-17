
using UnityEngine;

[RequireComponent(typeof(PrometeoCarController))]
public class SteeringSpeedScaler : MonoBehaviour
{
    private PrometeoCarController carController;

    public float maxSteeringSpeed = 1.0f;  // The maximum speed the wheels can turn when the car is stationary or at low speed.
    public float minSteeringSpeed = 0.3f;  // The minimum speed the wheels can turn when the car is at maximum speed.

    void Start()
    {
        carController = GetComponent<PrometeoCarController>();
    }

    void FixedUpdate()
    {
        ScaleSteeringSpeed();
    }

    private void ScaleSteeringSpeed()
    {
        // Calculate the scaled steering speed based on the current speed as a percentage of the max speed.
        float speedPercentage = carController.carSpeed / carController.maxSpeed;
        // At max speed, we want to use the minimum steering speed, and at low speeds, we want to use the maximum steering speed.
        carController.steeringSpeed = Mathf.Lerp(maxSteeringSpeed, minSteeringSpeed, speedPercentage);
    }
}
