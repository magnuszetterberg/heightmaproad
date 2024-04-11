
using UnityEngine;
using M2MqttUnity.Examples;
using System.Collections.Generic;

public class MQTTTest : MonoBehaviour
{
    public M2MqttUnityTest client; // Reference to your existing MQTT client script
    public GameObject prefab; // Assign the prefab you want to clone in the inspector
    public PlayerID playerID; // Reference to the PlayerID component
    public float smoothFactor = 0.1f; // Adjust this value for smoother or faster transitions

    private Dictionary<int, PositionData> pathData = new Dictionary<int, PositionData>();
    private Dictionary<int, GameObject> pathObjects = new Dictionary<int, GameObject>();

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
        if (client == null)
        {
            Debug.LogError("MQTT client script reference not set in the inspector.");
            return;
        }

        if (prefab == null)
        {
            Debug.LogError("Prefab for cloning is not assigned in the inspector.");
            return;
        }

        if (playerID == null)
        {
            Debug.LogError("PlayerID component reference not set in the inspector.");
            return;
        }

        client.OnMessage += OnMessage; // Register to the OnMessage event of M2MqttUnityTest
    }

    void Update()
    {
        foreach (var kvp in pathData)
        {
            int id = kvp.Key;
            PositionData data = kvp.Value;

            if (!pathObjects.TryGetValue(id, out GameObject obj))
            {
                // If there is no object for this ID, instantiate one and add it to the dictionary
                obj = Instantiate(prefab);
                pathObjects[id] = obj;
            }

            // Smoothly interpolate the position and rotation for each object
            Vector3 targetPosition = new Vector3(data.x, data.y, data.z);
            Quaternion targetRotation = Quaternion.Euler(data.pitch, data.yaw, data.roll);

            obj.transform.position = Vector3.Lerp(obj.transform.position, targetPosition, smoothFactor);
            obj.transform.rotation = Quaternion.Slerp(obj.transform.rotation, targetRotation, smoothFactor);
        }
    }



    void OnMessage(string topic, string msg)
    {
        //Debug.Log("Received message on topic: " + topic); // Log every message received for debugging

        string[] topicParts = topic.Split('/');
        if (topicParts.Length >= 3 && int.TryParse(topicParts[1], out int playerId))
        {
            if (playerId == playerID.UniqueID)
            {
               return;
               // Debug.Log("Received my own message, ignoring: " + msg);
            }
            else
            {
                //Debug.Log("Received other player's message: " + msg);

            PositionData receivedData = JsonUtility.FromJson<PositionData>(msg);
            
            // Update or create a new entry for this player ID
            pathData[playerId] = receivedData;

            // Ensure there's an instantiated object for this ID
            if (!pathObjects.ContainsKey(playerId))
            {
                GameObject newObj = Instantiate(prefab);
                pathObjects[playerId] = newObj;
                newObj.name = "Player_" + playerId; // Optional: Set the name to easily identify in the hierarchy
            }
        }
    }
}



     void OnApplicationQuit()
    {
        client.OnMessage -= OnMessage; // Unregister from the OnMessage event when quitting
    }
}
