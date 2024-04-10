
using UnityEngine;
using M2MqttUnity.Examples;

public class ObjectDataPublisher : MonoBehaviour
{
    public M2MqttUnityTest mqttClientScript;
    public string topic = "car/transform";

    // Update is called once per frame
    void Update()
    {
        PublishTransformData();
    }



private void PublishTransformData()
{
    if (mqttClientScript != null && mqttClientScript.IsConnected)
    {
        Vector3 position = transform.position;
        Vector3 rotation = transform.eulerAngles; // Unity uses Euler angles in degrees

        // Create a new instance of TransformData with formatted strings
        TransformData data = new TransformData
        {
            x = position.x.ToString("F2"),
            y = position.y.ToString("F2"),
            z = position.z.ToString("F2"),
            pitch = rotation.x.ToString("F2"),
            yaw = rotation.y.ToString("F2"),
            roll = rotation.z.ToString("F2")
        };

        string jsonData = JsonUtility.ToJson(data);

        mqttClientScript.PublishMessage(topic, jsonData);
    }
    else
    {
        Debug.LogWarning("MQTT client is not connected or not assigned.");
    }
}

[System.Serializable]
private class TransformData
{
    public string x;
    public string y;
    public string z;
    public string pitch;
    public string yaw;
    public string roll;
}
}
