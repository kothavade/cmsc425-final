using UnityEngine;         
using UnityEngine.UI;     


public class PlayerHealthSystem : MonoBehaviour
{
    private PlayerStats playerStats; // Reference to the player's health and stat logic

    [Header("Health Bar UI")]
    public Slider healthBarSlider; // The UI Slider that shows player's health visually

    [Header("Game Over UI")]
    public GameObject gameOverPanel; // UI panel that shows when the player dies

    [Header("Player Control Scripts")]
    public NewMovement movementScript;  // Reference to the player movement script to disable on death
    public NewCamera cameraScript;      // Reference to the camera control script to disable on death

    // Variables to manage enemy damage timing
    private float lastEnemyHitTime = -999f; // Time of last damage taken by enemy
    public float enemyDamageCooldown = 2f;  // Cooldown time (in seconds) between damage ticks
    public float enemyDamageAmount = 10f;   // Amount of damage the player takes when hit

    
    void Start()
    {
        // Get the PlayerStats component from the same GameObject
        playerStats = GetComponent<PlayerStats>();

        // If the component is missing, log an error and stop
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats not found on Player!");
            return;
        }

        // Initialize health values
        playerStats.currentHealth = playerStats.maxHealth = 100f;

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

    
    void Update()
    {
        // Update the health bar slider to reflect current health
        if (healthBarSlider != null)
        {
            healthBarSlider.value = playerStats.currentHealth;
        }

        // Check if player is dead
        if (playerStats.currentHealth <= 0)
        {
            Debug.Log("Game Over! Player is dead.");

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

            // Stop running Update() after this frame
            enabled = false;
        }
    }

    // Called when the player enters a trigger collider
    private void OnTriggerEnter(Collider other)
    {
        // Get the parent of the object (used for SpawnPoint checks)
        Transform parent = other.transform.parent;

        // If it's a Max Health Pickup SpawnPoint
        if (other.CompareTag("SpawnPoint") && parent != null && parent.name == "Max Health Spawner")
        {
            playerStats.ModifyMaxHealth(2f); // Increase max health by 2
            Debug.Log($"+2 Max Health! Max: {playerStats.maxHealth}, Current: {playerStats.currentHealth}");

            // Update the slider's max value
            if (healthBarSlider != null)
            {
                healthBarSlider.maxValue = playerStats.maxHealth;
            }
        }

        // If it's a current health pickup
        if (other.name.Contains("Current Health Pickup"))
        {
            playerStats.ModifyCurrentHealth(5f); // Heal player by 5
            Debug.Log($"+5 Current Health! Max: {playerStats.maxHealth}, Current: {playerStats.currentHealth}");

            // Remove the pickup from the game
            Destroy(other.gameObject);
        }

        // Handle first contact with an enemy
        TryEnemyDamage(other);
    }

    // Called while staying inside a trigger collider
    private void OnTriggerStay(Collider other)
    {
        // Keep applying enemy damage at cooldown intervals
        TryEnemyDamage(other);
    }

    // Applies damage if the player is touching an enemy and cooldown has passed
    private void TryEnemyDamage(Collider other)
    {
        // Check if the object is tagged "Enemy" and enough time has passed
        if (other.CompareTag("Enemy") && Time.time - lastEnemyHitTime >= enemyDamageCooldown)
        {
            playerStats.TakeDamage(enemyDamageAmount); // Deal damage to player
            lastEnemyHitTime = Time.time;              // Reset cooldown timer
            Debug.Log($"Enemy touched player -{enemyDamageAmount} Health");
        }
    }
}
