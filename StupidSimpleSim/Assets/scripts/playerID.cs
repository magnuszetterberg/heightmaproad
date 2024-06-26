
using UnityEngine;

public class PlayerID : MonoBehaviour
{
    public int UniqueID { get; private set; }

    void Awake()
    {
        // Generate a random 4-digit number when the script awakes
        UniqueID = UnityEngine.Random.Range(100000, 999999);
        Debug.Log("My Player ID: " + UniqueID); // Print this player's ID
    }
}
