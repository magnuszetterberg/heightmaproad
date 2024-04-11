
using UnityEngine;
using M2MqttUnity.Examples;
using System;

public class ObjectDataPublisher : MonoBehaviour
{
    public M2MqttUnityTest mqttClientScript;
    private string baseTopic = "cars/";
    private string fullTopic;
    private PlayerID playerID;

    void Start()
    {
        playerID = GetComponent<PlayerID>();
        if (playerID == null)
        {
            Debug.LogError("PlayerID component not found on the GameObject.");
            return;
        }

        // Use the player's unique ID to build the full MQTT topic
        fullTopic = baseTopic + playerID.UniqueID.ToString("D4") + "/transform"; // D4 ensures it's always 4 digits

        if (mqttClientScript == null)
        {
            Debug.LogError("MQTT client script reference not set in the inspector.");
            return;
        }

        if (!mqttClientScript.IsConnected)
        {
            Debug.LogWarning("MQTT client is not connected or not assigned.");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        PublishTransformData();
    }


private void PublishTransformData()
{
    if (mqttClientScript == null)
    {
        Debug.LogError("MQTT client script reference not set.");
        return;
    }

    if (!mqttClientScript.IsConnected)
    {
        Debug.LogWarning("MQTT client is not connected or not assigned.");
        return;
    }

    try
    {
        Vector3 position = transform.position;
        Vector3 rotation = transform.eulerAngles; // Unity uses Euler angles in degrees

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
        mqttClientScript.PublishMessage(fullTopic, jsonData);
    }
    catch (Exception ex)
    {
        Debug.LogError("Error publishing MQTT message: " + ex.Message);
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
