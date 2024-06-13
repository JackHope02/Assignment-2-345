using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // This function gets called when the Start button is clicked
    public void StartGame()
    {
        // Load the scene that contains your game.
        // Replace "GameScene" with the actual name of your game scene.
        SceneManager.LoadScene("Level1");
    }

    // This function gets called when the Quit button is clicked
    public void QuitGame()
    {
        // Log message to the console
        Debug.Log("Quit game request");

        // Close the game if running standalone build
        Application.Quit();

    }
}
