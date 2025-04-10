using UnityEngine;
using UnityEngine.UI;  

public class PlayerDeath : MonoBehaviour
{
    public PlayerStats playerStats;
    public GameObject gameOverPanel; // UI panel that shows when the player dies
    public Slider healthBarSlider; // The UI Slider that shows player's health visually
    public NewMovement movementScript;  // Reference to the player movement script to disable on death
    public NewCamera cameraScript;      // Reference to the camera control script to disable on death

    void Start()
    {
        // If the component is missing, log an error and stop
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats not found on Player!");
            return;
        }
        // Set up the health bar slider if it's assigned
        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = playerStats.maxHealth;
            healthBarSlider.value = playerStats.currentHealth;
        }

        // Hide the game over UI at the start
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        checkDeath();
    }

    void checkDeath()
    {
        if (playerStats.currentHealth <= 0) {
            
            Debug.Log("Player died");
            // Show the Game Over panel if assigned
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }

            // Disable player movement
            if (movementScript != null)
                movementScript.enabled = false;

            // Disable camera look
            if (cameraScript != null)
                cameraScript.enabled = false;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
    }
}
