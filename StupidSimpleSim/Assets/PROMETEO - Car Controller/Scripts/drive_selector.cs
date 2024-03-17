
using UnityEngine;

[RequireComponent(typeof(PrometeoCarController))]
public class DriveTypeController : MonoBehaviour
{
    public enum DriveType
    {
        FrontWheelDrive,
        RearWheelDrive,
        AllWheelDrive
    }

    private PrometeoCarController carController;
    public DriveType driveType;

    void Start()
    {
        carController = GetComponent<PrometeoCarController>();
    }

    // Call this method to apply torque to the correct wheels based on the selected drive type.
    public void ApplyDrive(float torque, float brakeForce)
    {
        switch (driveType)
        {
            case DriveType.FrontWheelDrive:
                ApplyFrontWheelDrive(torque, brakeForce);
                break;
            case DriveType.RearWheelDrive:
                ApplyRearWheelDrive(torque, brakeForce);
                break;
            case DriveType.AllWheelDrive:
                ApplyAllWheelDrive(torque, brakeForce);
                break;
        }
    }

    private void ApplyFrontWheelDrive(float torque, float brakeForce)
    {
        carController.frontLeftCollider.motorTorque = torque;
        carController.frontRightCollider.motorTorque = torque;
        ApplyBrakeTorque(brakeForce);
    }

    private void ApplyRearWheelDrive(float torque, float brakeForce)
    {
        carController.rearLeftCollider.motorTorque = torque;
        carController.rearRightCollider.motorTorque = torque;
        ApplyBrakeTorque(brakeForce);
    }

    private void ApplyAllWheelDrive(float torque, float brakeForce)
    {
        carController.frontLeftCollider.motorTorque = torque;
        carController.frontRightCollider.motorTorque = torque;
        carController.rearLeftCollider.motorTorque = torque;
        carController.rearRightCollider.motorTorque = torque;
        ApplyBrakeTorque(brakeForce);
    }

    private void ApplyBrakeTorque(float brakeForce)
    {
        // Assuming braking applies to all wheels.
        carController.frontLeftCollider.brakeTorque = brakeForce;
        carController.frontRightCollider.brakeTorque = brakeForce;
        carController.rearLeftCollider.brakeTorque = brakeForce;
        carController.rearRightCollider.brakeTorque = brakeForce;
    }

    // Call this method to set the drive type from another script or UI.
    public void SetDriveType(DriveType newDriveType)
    {
        driveType = newDriveType;
    }
}
