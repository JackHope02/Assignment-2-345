using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;
    public PlayerRespawn playerRespawn; // Reference to the PlayerRespawn script

    void Start()
    {
        currentHealth = maxHealth;

    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"Player takes {damage} damage, current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Debug.Log("Player Died");
            Die(); // Handle the player's death
        }
    }

    private void Die()
    {
        Debug.Log("Player has died. Respawning...");
        // Call the respawn method from the PlayerRespawn script
        if (playerRespawn != null)
        {
            playerRespawn.RespawnPlayer();
        }
        else
        {
            Debug.LogError("PlayerRespawn script not found!");
        }
    }

    // Reset the player's health to maximum
    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
}
