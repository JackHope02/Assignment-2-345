using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public Transform respawnPoint; // Drag the RespawnPoint GameObject here
    public PlayerHealth playerHealth; // Reference to the PlayerHealth script

    // Method to respawn the player at the designated respawn point
    public void RespawnPlayer()
    {
        Debug.Log("Respawning player at the respawn point.");
        transform.position = respawnPoint.position; // Move the player to the respawn point

        // Optionally reset the player's velocity if using Rigidbody
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            Debug.Log("Player's velocity reset.");
        }

        // Reset player health if needed
        if (playerHealth != null)
        {
            playerHealth.ResetHealth();
            Debug.Log("Player's health reset.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the player hits a hazard
        if (other.CompareTag("Hazard"))
        {
            RespawnPlayer();
        }
    }
}
