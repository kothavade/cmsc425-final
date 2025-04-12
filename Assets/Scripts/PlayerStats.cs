using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Base Stats")]
    public float maxHealth = 100f;
    public float currentHealth;
    public float moveSpeed = 5f;
    public float strength = 10f;
    public float defense = 5f;
    public float jumpForce = 30f;
    public float ExpPickupRange = 3f;
    public SphereCollider ExpPickupSphereCollider;
    public int Exp = 0;
    public int ExpToLevel = 50;
    public int Level = 1;

    [Header("Temporary Boosts")]
    private Dictionary<string, Coroutine> activeBoosts = new();

    private void Start()
    {
        currentHealth = maxHealth;
        ExpPickupSphereCollider.radius = ExpPickupRange;
    }

    public void TakeDamage(float damage)
    {
        float effectiveDamage = damage - defense;
        effectiveDamage = Mathf.Max(0, effectiveDamage);
        currentHealth -= effectiveDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"Took {effectiveDamage} damage. Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Debug.Log("Player is dead!");
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
        currentHealth = Mathf.Max(currentHealth, maxHealth);
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

    public void ModifyExpPickupRange(float amount)
    {
        ExpPickupRange += amount;
        ExpPickupRange = Mathf.Max(ExpPickupRange, 1f);
        ExpPickupSphereCollider.radius = ExpPickupRange;
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

    public void LevelUp()
    {
        Level += 1;
        ExpToLevel = (int)Mathf.Pow(2, Level);
        // allow player to choose upgrade

    }
    #endregion

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

        }

        activeBoosts[statName] = null;
    }

    #endregion
}