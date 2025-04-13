using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Base Stats")]
    [Tooltip("Maximum health points the player can have")]
    public float maxHealth = 100f;

    [Tooltip("Current health points of the player")]
    public float currentHealth;

    [Tooltip("Base movement speed of the player")]
    public float moveSpeed = 5f;

    [Tooltip("Player's strength stat, affects damage dealt")]
    public float strength = 10f;

    [Tooltip("Player's defense stat, reduces damage taken")]
    public float defense = 5f;

    [Tooltip("Force applied when player jumps")]
    public float jumpForce = 30f;

    [Tooltip("Radius in which player automatically collects experience points")]
    public float commonPickupRange = 3f;

    [Header("Upgrade System")]
    [Tooltip("All possible upgrades the player can receive")]
    public List<Upgrade> possibleUpgrades = new List<Upgrade>();

    [Tooltip("How many upgrade options to present per level")]
    public int upgradeOptionsPerLevel = 3;

    [Tooltip("Reference to the upgrade selection UI panel")]
    public UpgradeSelectionUI upgradeSelectionUI;

    [SerializeField]
    [Tooltip("Factor by which experience requirement increases per level")]
    private float expRequirementIncreaseFactor = 2f;

    [Tooltip("Sphere collider used for automatic experience pickup")]
    public SphereCollider ExpPickupSphereCollider;

    [Tooltip("Current experience points")]
    public int Exp = 0;

    [Tooltip("Experience points required to level up")]
    public int ExpToLevel = 1;

    [Tooltip("Current player level")]
    public int Level = 1;

    [Header("Temporary Boosts")]
    [Tooltip("Dictionary of active stat boost coroutines")]
    private Dictionary<string, Coroutine> activeBoosts = new();

    [Header("Audio")]
    [Tooltip("Sound played when player takes damage")]
    public AudioClip damageSound;
    public AudioSource audioSource;

    [Header("Misc")]
    public float upgradeSelectionTimeScale = .05f;


    private void Start()
    {
        currentHealth = maxHealth;
        ExpPickupSphereCollider.radius = commonPickupRange;
    }

    public void TakeDamage(float damage)
    {

        float effectiveDamage = damage - defense;
        effectiveDamage = Mathf.Max(0, effectiveDamage);
        HandleDamageEffects(effectiveDamage);

        currentHealth -= effectiveDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"Took {effectiveDamage} damage. Current health: {currentHealth}");


        if (currentHealth <= 0)
        {
            Debug.Log("Player is dead!");
        }
    }

    private void HandleDamageEffects(float effectiveDamage)
    {
        if (effectiveDamage > 0 && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }
    }

    #region Permanent Stat Modifications
    public void ModifyMaxHealth(float amount)
    {
        maxHealth += amount;
        maxHealth = Mathf.Max(1, maxHealth);
        Debug.Log($"Max Health changed by {amount}. Max health: {maxHealth}");
    }

    public void ModifyCurrentHealth(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        Debug.Log($"Current Health changed by {amount}. Current Health: {currentHealth}");
    }

    public void ModifySpeed(float amount)
    {
        moveSpeed += amount;
        moveSpeed = Mathf.Max(0.1f, moveSpeed); // Ensure speed doesn't go below 0.1
        Debug.Log($"Speed changed by {amount}. Current speed: {moveSpeed}");
    }

    public void ModifyStrength(float amount)
    {
        strength += amount;
        strength = Mathf.Max(0, strength);
        Debug.Log($"Strength changed by {amount}. Current strength: {strength}");
    }

    public void ModifyDefense(float amount)
    {
        defense += amount;
        defense = Mathf.Max(0, defense);
        Debug.Log($"Defense changed by {amount}. Current defense: {defense}");
    }

    public void ModifyJump(float amount)
    {
        jumpForce += amount;
        jumpForce = Mathf.Max(0, jumpForce);
        Debug.Log($"Jump Force changed by {amount}. Current jumpForce: {jumpForce}");

    }

    public void ModifyCommonPickupRange(float amount)
    {
        commonPickupRange += amount;
        commonPickupRange = Mathf.Max(commonPickupRange, 1f);
        ExpPickupSphereCollider.radius = commonPickupRange;
    }
    public void ModifyExp(int amount)
    {
        Exp += amount;
        if (Exp >= ExpToLevel)
        {
            Exp -= ExpToLevel;
            LevelUp();

        }
    }

    #endregion
    public void LevelUp()
    {
        Level += 1;
        ExpToLevel = (int)(expRequirementIncreaseFactor * Level);

        // Show upgrade selection UI
        PresentUpgradeOptions();

        // Pause game
        Time.timeScale = upgradeSelectionTimeScale;

    }

    private void PresentUpgradeOptions()
    {
        if (upgradeSelectionUI == null)
        {
            Debug.LogError("Upgrade Selection UI reference is missing");
            return;
        }

        // Get random selection of upgrades
        List<Upgrade> selectedUpgrades = GetRandomUpgrades(upgradeOptionsPerLevel);

        Debug.Log("Attempting to show upgrade panel");
        upgradeSelectionUI.gameObject.SetActive(true);
        upgradeSelectionUI.ShowUpgradeOptions(selectedUpgrades, OnUpgradeSelected);
    }

    private List<Upgrade> GetRandomUpgrades(int count)
    {
        // Create a copy of the possible upgrades list to avoid modifying the original
        List<Upgrade> availableUpgrades = new List<Upgrade>(possibleUpgrades);
        List<Upgrade> selectedUpgrades = new List<Upgrade>();

        // Select random upgrades
        int selectionCount = Mathf.Min(count, availableUpgrades.Count);
        for (int i = 0; i < selectionCount; i++)
        {
            int randomIndex = Random.Range(0, availableUpgrades.Count);
            selectedUpgrades.Add(availableUpgrades[randomIndex]);
            availableUpgrades.RemoveAt(randomIndex);
        }

        return selectedUpgrades;
    }

    public void OnUpgradeSelected(Upgrade selectedUpgrade)
    {
        Debug.Log("OnUpgradeSelected called with: " + selectedUpgrade.upgradeName);

        // Apply the selected upgrade
        ApplyUpgrade(selectedUpgrade);

        // Resume game
        Time.timeScale = 1f;

        // Hide panel
        upgradeSelectionUI.gameObject.SetActive(false);
    }

    // This will need to be handled differently eventually
    private void ApplyUpgrade(Upgrade upgrade)
    {
        // Apply the appropriate stat boost based on upgrade type
        switch (upgrade.type)
        {
            case Upgrade.UpgradeType.MaxHealth:
                ModifyMaxHealth(upgrade.value);
                ModifyCurrentHealth(upgrade.value);
                break;
            case Upgrade.UpgradeType.Speed:
                ModifySpeed(upgrade.value);
                break;
            case Upgrade.UpgradeType.Strength:
                ModifyStrength(upgrade.value);
                break;
            case Upgrade.UpgradeType.Defense:
                ModifyDefense(upgrade.value);
                break;
            case Upgrade.UpgradeType.JumpForce:
                ModifyJump(upgrade.value);
                break;
            case Upgrade.UpgradeType.CommonPickupRange:
                ModifyCommonPickupRange(upgrade.value);
                break;
        }

        Debug.Log($"Applied upgrade: {upgrade.upgradeName}");
    }

    public float GetExpPercentage()
    {
        return (float)Exp / ExpToLevel;
    }
    public int GetLevel()
    {
        return Level;
    }

    #region Temporary Stat Boosts

    public void ApplyTemporaryHealthBoost(float amount, float duration)
    {
        // Stop any active health boost
        StopBoostIfActive("health");

        // Start a new health boost
        activeBoosts["health"] = StartCoroutine(TemporaryStatBoost("health", amount, duration));
    }

    public void ApplyTemporarySpeedBoost(float amount, float duration)
    {
        StopBoostIfActive("speed");
        activeBoosts["speed"] = StartCoroutine(TemporaryStatBoost("speed", amount, duration));
    }

    public void ApplyTemporaryStrengthBoost(float amount, float duration)
    {
        StopBoostIfActive("strength");
        activeBoosts["strength"] = StartCoroutine(TemporaryStatBoost("strength", amount, duration));
    }

    public void ApplyTemporaryDefenseBoost(float amount, float duration)
    {
        StopBoostIfActive("defense");
        activeBoosts["defense"] = StartCoroutine(TemporaryStatBoost("defense", amount, duration));
    }

    public void ApplyTemporaryJumpBoost(float amount, float duration)
    {
        StopBoostIfActive("jump");
        activeBoosts["jump"] = StartCoroutine(TemporaryStatBoost("jump", amount, duration));
    }
    public void ApplyTemporaryCommonPickupRangeBoost(float amount, float duration)
    {
        StopBoostIfActive("common pickup range");
        activeBoosts["common pickup range"] = StartCoroutine(TemporaryStatBoost("common pickup range", amount, duration));
    }

    private void StopBoostIfActive(string statName)
    {
        if (activeBoosts.ContainsKey(statName) && activeBoosts[statName] != null)
        {
            StopCoroutine(activeBoosts[statName]);
            activeBoosts[statName] = null;
        }
    }

    private IEnumerator TemporaryStatBoost(string statName, float amount, float duration)
    {
        // Apply the boost
        switch (statName)
        {
            case "health":
                ModifyMaxHealth(amount);
                Debug.Log($"Applied temporary health boost of {amount} for {duration} seconds");
                break;
            case "speed":
                moveSpeed += amount;
                Debug.Log($"Applied temporary speed boost of {amount} for {duration} seconds");
                break;
            case "strength":
                strength += amount;
                Debug.Log($"Applied temporary strength boost of {amount} for {duration} seconds");
                break;
            case "defense":
                defense += amount;
                Debug.Log($"Applied temporary defense boost of {amount} for {duration} seconds");
                break;
            case "jump":
                jumpForce += amount;
                Debug.Log($"Applied temporary jump boost of {amount} for {duration} seconds");
                break;
            case "common pickup range":
                ModifyCommonPickupRange(amount);
                Debug.Log($"Applied temporary common pickup range boost of {amount} for {duration} seconds");
                break;
        }

        // Wait for the duration
        yield return new WaitForSeconds(duration);

        // Remove the boost
        switch (statName)
        {
            case "health":
                // For health, we don't typically remove health after a boost
                break;
            case "speed":
                moveSpeed -= amount;
                Debug.Log($"Speed boost of {amount} ended. Current speed: {moveSpeed}");
                break;
            case "strength":
                strength -= amount;
                Debug.Log($"Strength boost of {amount} ended. Current strength: {strength}");
                break;
            case "defense":
                defense -= amount;
                Debug.Log($"Defense boost of {amount} ended. Current defense: {defense}");
                break;
            case "jump":
                jumpForce -= amount;
                Debug.Log($"Jump boost of {amount} ended. Current jump: {jumpForce}");
                break;
            case "common pickup range":
                ModifyCommonPickupRange(-amount);
                Debug.Log($"Common pickup range boost of {amount} ended. Current cpr: {commonPickupRange}");
                break;

        }

        activeBoosts[statName] = null;
    }

    #endregion
}