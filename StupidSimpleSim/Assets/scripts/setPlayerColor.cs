using UnityEngine;

public class PlayerColor : MonoBehaviour
{
    public PlayerID playerID; // Make this public

    void Start()
    {
        if (playerID != null)
        {
            // Use the UniqueID to set the color
            SetColorFromPlayerID(playerID.UniqueID);
        }
        else
        {
            Debug.LogError("PlayerID component not found!");
        }
    }

    public void SetColorFromPlayerID(int playerID)
    {
        // Normalize playerID by converting it to a value between 0 and 1
        float normalizedID = (playerID % 1000000) / 999999f;

        // Construct a color using the normalizedID
        Color playerColor = new Color(normalizedID, 1.0f - normalizedID, 0.5f);

        // Find the 'Body' child object and its MeshRenderer component
        Transform bodyTransform = transform.Find("Body"); // Ensure 'Body' is the exact name of the child
        if(bodyTransform != null)
        {
            Debug.Log("Found 'Body' GameObject as a child of the car controller.");
            MeshRenderer bodyRenderer = bodyTransform.GetComponent<MeshRenderer>();
            if (bodyRenderer != null)
            {
                Debug.Log("Setting color for player ID: " + playerID);
                // Change the color of all materials on the 'Body'
                foreach (Material mat in bodyRenderer.materials)
                {
                    mat.color = playerColor;
                }
            }
            else
            {
                Debug.LogError("MeshRenderer not found on the 'Body' GameObject!");
            }
        }
        else
        {
            Debug.LogError("'Body' GameObject not found as a child of the car controller!");
        }
    }
}