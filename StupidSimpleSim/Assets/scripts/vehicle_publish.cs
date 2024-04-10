
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
            x = position.x.ToString("F1"),
            y = position.y.ToString("F1"),
            z = position.z.ToString("F1"),
            pitch = rotation.x.ToString("F1"),
            yaw = rotation.y.ToString("F1"),
            roll = rotation.z.ToString("F1")
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
