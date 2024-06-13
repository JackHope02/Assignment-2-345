using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    public int nextLevelBuildIndex; // The build index of the next level to load

    void OnTriggerEnter(Collider other)
    {
        // Check if the player enters the trigger
        if (other.CompareTag("Player"))
        {
            // Use SceneManager to load the next level
            SceneManager.LoadScene(nextLevelBuildIndex);
        }
    }
}