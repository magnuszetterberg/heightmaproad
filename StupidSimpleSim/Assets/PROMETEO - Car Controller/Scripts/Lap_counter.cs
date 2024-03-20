
using UnityEngine;
using UnityEngine.UI; // Include the UI namespace to work with Text elements.

public class LapCounter : MonoBehaviour
{
    private int currentCheckpoint = 0;
    private int lapCount = 0;
    private float lapTime = 0f;
    private float bestLapTime = Mathf.Infinity;

    // UI Text References
    public Text lapTimeText; // Assign this in the inspector
    public Text bestLapTimeText; // Assign this in the inspector
    public Text currentLapCountText; // Assign this in the inspector

    // This should be equal to the total number of checkpoints including the finish line.
    public int lastCheckpointNumber = 4; // Set this to the correct value in the inspector

    private void Update()
    {
        // Always update lap time when racing
        lapTime += Time.deltaTime;
        // Update the UI Text with the current lap time.
        if (lapTimeText != null)
        {
            lapTimeText.text = "Lap Time: " + lapTime.ToString("F2");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            Checkpoint checkpointScript = other.GetComponent<Checkpoint>();
            if (checkpointScript != null)
            {
                int checkpointNumber = checkpointScript.checkpointNumber;

                // Check if this is the next checkpoint in sequence
                if (checkpointNumber == currentCheckpoint + 1)
                {
                    currentCheckpoint = checkpointNumber;
                    Debug.Log("Checkpoint " + checkpointNumber + " passed!");
                }
                // Check if the player has hit the finish line
                if (checkpointNumber == lastCheckpointNumber && currentCheckpoint == lastCheckpointNumber)
                {
                    // Increment lap count
                    lapCount++;
                    // Update the UI Text with the current lap count.
                    if (currentLapCountText != null)
                    {
                        currentLapCountText.text = "Lap: " + lapCount.ToString();
                    }

                    // Output the completed lap time before resetting.
                    Debug.Log("Lap " + lapCount + " complete! Lap time: " + lapTime);

                    // Check if the completed lap time is a new record.
                    if (lapTime < bestLapTime)
                    {
                        bestLapTime = lapTime;
                        Debug.Log("New best lap time: " + bestLapTime);

                        // Update the UI Text with the best lap time.
                        if (bestLapTimeText != null)
                        {
                            bestLapTimeText.text = "Best Lap: " + bestLapTime.ToString("F2");
                        }
                    }

                    // Reset the current checkpoint and lap time for the next lap.
                    currentCheckpoint = 0;
                    lapTime = 0f;
                }
            }
        }
    }
}
