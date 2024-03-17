
using UnityEngine;

[RequireComponent(typeof(PrometeoCarController))]
public class CarPhysicsScaler : MonoBehaviour
{
    private PrometeoCarController carController;
    private Rigidbody carRigidbody;

    public float downforceFactor = 1.0f; // Increase to apply more downforce at higher speeds
    public float dragFactor = 0.01f; // Increase to apply more drag at higher speeds

    void Start()
    {
        carController = GetComponent<PrometeoCarController>();
        carRigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        ScaleCarPhysics();
    }

    private void ScaleCarPhysics()
    {
        if (carController && carRigidbody)
        {
            // Scale downforce based on speed
            Vector3 downforce = Vector3.down * downforceFactor * carController.carSpeed;
            carRigidbody.AddForce(downforce, ForceMode.Force);

            // Scale drag based on speed
            float speedPercentage = carController.carSpeed / carController.maxSpeed;
            carRigidbody.drag = Mathf.Lerp(carRigidbody.drag, speedPercentage * dragFactor, Time.fixedDeltaTime);
        }
    }
}
