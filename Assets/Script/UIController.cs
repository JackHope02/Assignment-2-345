using UnityEngine;
using TMPro; // Make sure you have this using directive to use TextMeshPro components

public class UIController : MonoBehaviour
{
    public TextMeshProUGUI instructionText; // Reference to the TextMeshProUGUI component
    public float visibleTime = 5.0f; // Time in seconds for which the instructions are visible

    private float timer;

    void Start()
    {
        timer = visibleTime;
    }

    void Update()
    {
        // Hide the text after the initial visible time
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            instructionText.enabled = false;
        }

        // Hide text when TAB is pressed for the first time
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            instructionText.enabled = false;
        }
    }
}
