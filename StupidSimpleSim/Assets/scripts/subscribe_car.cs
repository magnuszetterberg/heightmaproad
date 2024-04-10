
using UnityEngine;
using M2MqttUnity.Examples;

public class MQTTTest : MonoBehaviour
{
    public string topic = "rosi_integration/system/position";
    public M2MqttUnityTest client;
    public float smoothFactor = 0.1f; // Adjust this value for smoother or faster transitions

    private PositionData targetPositionData;
    private bool isTransitioning = false;

    [System.Serializable]
    public class PositionData
    {
        public float x;
        public float y;
        public float z;
        public float pitch;
        public float yaw;
        public float roll;
    }

    void Start()
    {
        client.OnMessage += OnMessage;
        targetPositionData = new PositionData();
    }

    void Update()
    {
        if (isTransitioning)
        {
            // Smoothly interpolate the position
            Vector3 targetPosition = new Vector3(targetPositionData.x, targetPositionData.y, targetPositionData.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothFactor);

            // Smoothly interpolate the rotation
            Quaternion targetRotation = Quaternion.Euler(targetPositionData.pitch, targetPositionData.yaw, targetPositionData.roll);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothFactor);

            // Optionally, you can stop interpolating once you're close enough to the target
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f && 
                Quaternion.Angle(transform.rotation, targetRotation) < 1f)
            {
                isTransitioning = false; // Stop interpolating when the target is reached within a threshold
            }
        }
    }

    void OnMessage(string topic, string msg)
    {
        if (topic == this.topic)
        {
            Debug.Log("Received message from " + topic + " : " + msg);

            // Parse the incoming data and store it as the new target for interpolation
            targetPositionData = JsonUtility.FromJson<PositionData>(msg);
            isTransitioning = true; // Start transitioning towards the new target position and rotation
        }
    }

    void OnApplicationQuit()
    {
        client.OnMessage -= OnMessage; // Unsubscribe from the event when quitting
    }
}
