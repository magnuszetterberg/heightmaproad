
using UnityEngine;
using UnityEngine.UI;

public class DisplayPlayerID : MonoBehaviour
{
    public PlayerID playerID; // Reference to the PlayerID component
    public Text idText; // Reference to the UI Text component

    void Start()
    {
        if (playerID == null)
        {
            Debug.LogError("PlayerID component reference not set in the inspector.");
            return;
        }

        if (idText == null)
        {
            Debug.LogError("UI Text component reference not set in the inspector.");
            return;
        }

        // Set the text to display the player's unique ID
        idText.text = "Player: " + playerID.UniqueID.ToString();
    }
}
