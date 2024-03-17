
using UnityEngine;

[RequireComponent(typeof(PrometeoCarController))]
public class TireFrictionScaler : MonoBehaviour
{
    private PrometeoCarController carController;

    public float frictionFactor = 0.5f; // Adjust this value to set how much friction scales with speed.

    private WheelFrictionCurve originalFrictionCurveFront;
    private WheelFrictionCurve originalFrictionCurveRear;

    void Start()
    {
        carController = GetComponent<PrometeoCarController>();

        // Store the original friction curves at the start
        originalFrictionCurveFront = carController.frontLeftCollider.sidewaysFriction;
        originalFrictionCurveRear = carController.rearLeftCollider.sidewaysFriction;
    }

    void FixedUpdate()
    {
        ScaleTireFriction();
    }

    private void ScaleTireFriction()
    {
        // Calculate the scaled friction based on the current speed as a percentage of the max speed.
        float speedPercentage = carController.carSpeed / carController.maxSpeed;
        float scaledFrictionStiffness = Mathf.Lerp(originalFrictionCurveFront.stiffness, originalFrictionCurveFront.stiffness + frictionFactor, speedPercentage);

        // Apply the scaled friction to the sideways friction curve of each wheel.
        WheelFrictionCurve newFrictionCurve;

        // Front wheels
        newFrictionCurve = carController.frontLeftCollider.sidewaysFriction;
        newFrictionCurve.stiffness = scaledFrictionStiffness;
        carController.frontLeftCollider.sidewaysFriction = newFrictionCurve;
        carController.frontRightCollider.sidewaysFriction = newFrictionCurve;

        // Rear wheels
        newFrictionCurve = carController.rearLeftCollider.sidewaysFriction;
        newFrictionCurve.stiffness = scaledFrictionStiffness;
        carController.rearLeftCollider.sidewaysFriction = newFrictionCurve;
        carController.rearRightCollider.sidewaysFriction = newFrictionCurve;
    }
}
