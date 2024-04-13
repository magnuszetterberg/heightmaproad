
using UnityEngine;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour
{
    public InputField inputField;
    public Button submitButton;
    public TerrainGenerator terrainGenerator;

    private void Start()
    {
        submitButton.onClick.AddListener(SubmitValue);
    }

    private void SubmitValue()
    {
        if (inputField.text.Length == 4 && int.TryParse(inputField.text, out int seed))
        {
            terrainGenerator.SetSeedAndGenerate(seed);
        }
        else
        {
            Debug.LogError("Please enter a 4-digit value.");
        }
    }
}
