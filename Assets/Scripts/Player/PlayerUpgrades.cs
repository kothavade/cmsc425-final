using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUpgrades : MonoBehaviour
{
    [Header("Upgrade System")]
    [Tooltip("All possible upgrades the player can receive. If left empty, upgrades will be loaded from UpgradeDatabase.")]
    public List<Upgrade> possibleUpgrades = new List<Upgrade>();

    [Tooltip("How many upgrade options to present per level")]
    public int upgradeOptionsPerLevel = 3;

    [Tooltip("Reference to the upgrade selection UI panel")]
    public UpgradeSelectionUI upgradeSelectionUI;

    [Tooltip("Time scale when upgrade selection UI is open")]
    public float upgradeSelectionTimeScale = 0.05f;

    [Tooltip("Whether to use the UpgradeDatabase for upgrades")]
    public bool useUpgradeDatabase = true;
    
    [Tooltip("Whether to use level-based weighted quality selection")]
    public bool useWeightedQualitySelection = true;

    private PlayerStats playerStats;

    private void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats component is missing!");
        }
        
        // Load upgrades from database if using the database and the list is empty
        if (useUpgradeDatabase && possibleUpgrades.Count == 0)
        {
            if (UpgradeDatabase.Instance != null)
            {
                possibleUpgrades = UpgradeDatabase.Instance.GetAllUpgrades();
            }
            else
            {
                Debug.LogWarning("UpgradeDatabase instance not found, but useUpgradeDatabase is true.");
            }
        }
    }

    public void HandleLevelUp()
    {
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
        // If we're using the upgrade database and it exists, get random upgrades from there
        if (useUpgradeDatabase && UpgradeDatabase.Instance != null)
        {
            if (useWeightedQualitySelection)
            {
                // Use weighted selection based on player level
                return UpgradeDatabase.Instance.GetRandomUpgradesWeighted(count, playerStats.GetLevel());
            }
            else
            {
                // Use completely random selection
                return UpgradeDatabase.Instance.GetRandomUpgrades(count);
            }
        }
        
        // Otherwise use the local list
        List<Upgrade> availableUpgrades = new List<Upgrade>(possibleUpgrades);
        List<Upgrade> selectedUpgrades = new List<Upgrade>();

        if (availableUpgrades.Count == 0)
        {
            Debug.LogError("No upgrades available to select from!");
            return selectedUpgrades;
        }

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
        string upgradeName = selectedUpgrade.useQualitySystem 
            ? $"{selectedUpgrade.quality} {selectedUpgrade.upgradeName}" 
            : selectedUpgrade.upgradeName;
            
        Debug.Log($"OnUpgradeSelected called with: {upgradeName}");

        // Apply the selected upgrade
        ApplyUpgrade(selectedUpgrade);

        // Resume game
        Time.timeScale = 1f;

        // Hide panel
        upgradeSelectionUI.gameObject.SetActive(false);
    }

    private void ApplyUpgrade(Upgrade upgrade)
    {
        if (playerStats == null)
        {
            Debug.LogError("Cannot apply upgrade: PlayerStats reference is missing!");
            return;
        }

        // Calculate adjusted value based on quality
        float adjustedValue = upgrade.GetAdjustedValue();
        
        // Apply the appropriate stat boost based on upgrade type
        switch (upgrade.type)
        {
            case Upgrade.UpgradeType.MaxHealth:
                playerStats.ModifyMaxHealth(adjustedValue);
                playerStats.ModifyCurrentHealth(adjustedValue);
                break;
            case Upgrade.UpgradeType.Speed:
                playerStats.ModifySpeed(adjustedValue);
                break;
            case Upgrade.UpgradeType.Strength:
                playerStats.ModifyStrength(adjustedValue);
                break;
            case Upgrade.UpgradeType.Defense:
                playerStats.ModifyDefense(adjustedValue);
                break;
            case Upgrade.UpgradeType.JumpForce:
                playerStats.ModifyJump(adjustedValue);
                break;
            case Upgrade.UpgradeType.CommonPickupRange:
                playerStats.ModifyCommonPickupRange(adjustedValue);
                break;
        }

        Debug.Log($"Applied {upgrade.quality} {upgrade.upgradeName} with adjusted value: {adjustedValue}");
    }
}