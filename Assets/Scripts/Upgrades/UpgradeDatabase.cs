using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeDatabase : MonoBehaviour
{
    // Singleton pattern to access the database from anywhere
    public static UpgradeDatabase Instance { get; private set; }

    [Header("Default Upgrades")]
    [Tooltip("Pre-defined upgrades that will be available in the game")]
    public List<Upgrade> defaultUpgrades = new List<Upgrade>();

    [Header("Upgrade Settings")]
    [Tooltip("Should the database initialize default upgrades if none are provided?")]
    public bool useAutoGeneratedUpgrades = true;

    [Header("Base Upgrade Values")]
    [Tooltip("Base values for each upgrade type")]
    [SerializeField] private float baseHealthValue = 20f;
    [SerializeField] private float baseSpeedValue = 1f;
    [SerializeField] private float baseStrengthValue = 5f;
    [SerializeField] private float baseDefenseValue = 3f;
    [SerializeField] private float baseJumpValue = 5f;
    [SerializeField] private float basePickupRangeValue = 1f;

    [Header("Quality Distribution Settings")]
    [Tooltip("Maximum player level for quality scaling (quality chances max out at this level)")]
    [SerializeField] private int maxLevelForQualityScaling = 30;

    [Tooltip("Base chance for Legendary quality at level 1")]
    [Range(0f, 1f)]
    [SerializeField] private float baseLegendaryChance = 0f;

    [Tooltip("Maximum chance for Legendary quality at max level")]
    [Range(0f, 1f)]
    [SerializeField] private float maxLegendaryChance = 0.1f;

    [Tooltip("Base chance for Epic quality at level 1")]
    [Range(0f, 1f)]
    [SerializeField] private float baseEpicChance = 0f;

    [Tooltip("Maximum chance for Epic quality at max level")]
    [Range(0f, 1f)]
    [SerializeField] private float maxEpicChance = 0.25f;

    [Tooltip("Base chance for Major quality at level 1")]
    [Range(0f, 1f)]
    [SerializeField] private float baseMajorChance = 0.05f;

    [Tooltip("Maximum chance for Major quality at max level")]
    [Range(0f, 1f)]
    [SerializeField] private float maxMajorChance = 0.35f;

    [Tooltip("Base chance for Normal quality at level 1")]
    [Range(0f, 1f)]
    [SerializeField] private float baseNormalChance = 0.25f;

    [Tooltip("Maximum chance for Normal quality at max level")]
    [Range(0f, 1f)]
    [SerializeField] private float maxNormalChance = 0.20f;

    [Tooltip("Default upgrade icon (will need to think about how to add appropriate icons later)")]
    [SerializeField] private Sprite defaultIcon;
    [SerializeField] private bool useDefaultIcon = true;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize the default upgrades if the list is empty and auto-generation is enabled
        if (defaultUpgrades.Count == 0 && useAutoGeneratedUpgrades)
        {
            InitializeDefaultUpgrades();
        }
    }

    private void InitializeDefaultUpgrades()
    {
        // Generate all qualities for each upgrade type

        // Health upgrades
        AddUpgradeWithAllQualities(
            "Health Boost",
            "Increases maximum health",
            Upgrade.UpgradeType.MaxHealth,
            baseHealthValue);

        // Speed upgrades
        AddUpgradeWithAllQualities(
            "Speed Increase",
            "Increases movement speed",
            Upgrade.UpgradeType.Speed,
            baseSpeedValue);

        // Strength upgrades
        AddUpgradeWithAllQualities(
            "Strength Training",
            "Increases strength",
            Upgrade.UpgradeType.Strength,
            baseStrengthValue);

        // Defense upgrades
        AddUpgradeWithAllQualities(
            "Defensive Stance",
            "Increases defense",
            Upgrade.UpgradeType.Defense,
            baseDefenseValue);

        // Jump upgrades
        AddUpgradeWithAllQualities(
            "Jump Enhancement",
            "Increases jump force",
            Upgrade.UpgradeType.JumpForce,
            baseJumpValue);

        // Pickup range upgrades
        AddUpgradeWithAllQualities(
            "Magnet Upgrade",
            "Increases pickup range",
            Upgrade.UpgradeType.CommonPickupRange,
            basePickupRangeValue);

    }

    // Will need to figure out appropriate icons
    private void AddUpgradeWithAllQualities(string name, string baseDescription, Upgrade.UpgradeType type, float baseValue)
    {
        // Create a version of the upgrade for each quality

        // Minor quality
        defaultUpgrades.Add(new Upgrade
        {
            upgradeName = name,
            description = $"{baseDescription} by a small amount",
            type = type,
            useQualitySystem = true,
            quality = Upgrade.UpgradeQuality.Minor,
            value = baseValue
        });

        // Normal quality
        defaultUpgrades.Add(new Upgrade
        {
            upgradeName = name,
            description = $"{baseDescription} by a moderate amount",
            type = type,
            useQualitySystem = true,
            quality = Upgrade.UpgradeQuality.Normal,
            value = baseValue
        });

        // Major quality
        defaultUpgrades.Add(new Upgrade
        {
            upgradeName = name,
            description = $"{baseDescription} by a large amount",
            type = type,
            useQualitySystem = true,
            quality = Upgrade.UpgradeQuality.Major,
            value = baseValue
        });

        // Epic quality
        defaultUpgrades.Add(new Upgrade
        {
            upgradeName = name,
            description = $"{baseDescription} by a very large amount",
            type = type,
            useQualitySystem = true,
            quality = Upgrade.UpgradeQuality.Epic,
            value = baseValue
        });

        // Legendary quality
        defaultUpgrades.Add(new Upgrade
        {
            upgradeName = name,
            description = $"{baseDescription} by an enormous amount",
            type = type,
            useQualitySystem = true,
            quality = Upgrade.UpgradeQuality.Legendary,
            value = baseValue
        });

        // Temporary
        if (useDefaultIcon)
        {
            foreach (Upgrade upgrade in defaultUpgrades)
            {
                upgrade.icon = defaultIcon;
            }
        }



    }

    // Might be useful later for unique weapon upgrades or abilities, but will likely need to be reworked.
    private void AddSpecialUpgrade(string name, string description, Upgrade.UpgradeType type, float value)
    {
        // Create an upgrade that doesn't use the quality system
        defaultUpgrades.Add(new Upgrade
        {
            upgradeName = name,
            description = description,
            type = type,
            useQualitySystem = false,
            value = value
        });
    }

    // Method to get all available upgrades
    public List<Upgrade> GetAllUpgrades()
    {
        return new List<Upgrade>(defaultUpgrades);
    }

    // Method to get upgrades of a specific type
    // Might be useful for curating upgrades selection by current player build
    public List<Upgrade> GetUpgradesByType(Upgrade.UpgradeType type)
    {
        List<Upgrade> filteredUpgrades = new List<Upgrade>();

        foreach (var upgrade in defaultUpgrades)
        {
            if (upgrade.type == type)
            {
                filteredUpgrades.Add(upgrade);
            }
        }

        return filteredUpgrades;
    }

    // Method to get upgrades of a specific quality
    public List<Upgrade> GetUpgradesByQuality(Upgrade.UpgradeQuality quality)
    {
        List<Upgrade> filteredUpgrades = new List<Upgrade>();

        foreach (var upgrade in defaultUpgrades)
        {
            if (upgrade.quality == quality)
            {
                filteredUpgrades.Add(upgrade);
            }
        }

        return filteredUpgrades;
    }

    // Method to get random upgrades
    public List<Upgrade> GetRandomUpgrades(int count)
    {
        if (defaultUpgrades.Count == 0)
            return new List<Upgrade>();

        List<Upgrade> availableUpgrades = new List<Upgrade>(defaultUpgrades);
        List<Upgrade> selectedUpgrades = new List<Upgrade>();

        int selectionCount = Mathf.Min(count, availableUpgrades.Count);
        for (int i = 0; i < selectionCount; i++)
        {
            int randomIndex = Random.Range(0, availableUpgrades.Count);
            selectedUpgrades.Add(availableUpgrades[randomIndex]);
            availableUpgrades.RemoveAt(randomIndex);
        }

        return selectedUpgrades;
    }

    // Method to get random upgrades with weighted quality selection
    // Higher player level increases chances of better quality upgrades
    public List<Upgrade> GetRandomUpgradesWeighted(int count, int playerLevel)
    {
        if (defaultUpgrades.Count == 0)
            return new List<Upgrade>();

        List<Upgrade> selectedUpgrades = new List<Upgrade>();

        for (int i = 0; i < count; i++)
        {
            // Determine quality based on player level
            Upgrade.UpgradeQuality quality = GetWeightedQuality(playerLevel);

            // Get all upgrades of that quality
            List<Upgrade> qualityUpgrades = GetUpgradesByQuality(quality);

            // If we have upgrades of this quality, pick one randomly
            if (qualityUpgrades.Count > 0)
            {
                int randomIndex = Random.Range(0, qualityUpgrades.Count);
                selectedUpgrades.Add(qualityUpgrades[randomIndex]);
            }
            else
            {
                // Fallback to any random upgrade if no upgrades of the desired quality exist
                int randomIndex = Random.Range(0, defaultUpgrades.Count);
                selectedUpgrades.Add(defaultUpgrades[randomIndex]);
            }
        }

        return selectedUpgrades;
    }

    // This method is a bit confusing, may not work exactly as intended, may need to rework.
    private Upgrade.UpgradeQuality GetWeightedQuality(int playerLevel)
    {
        // Calculate level factor (0.0 to 1.0) based on player level
        float levelFactor = Mathf.Min((float)playerLevel / maxLevelForQualityScaling, 1f);

        /*
        +-------+----------------------+----------------------+---------------------+-------------------+----------------------+
        | Level | Minor                | Normal               | Major               | Epic              | Legendary            |
        +-------+----------------------+----------------------+---------------------+-------------------+----------------------+
        | 1     | Remainder (~70%)     | baseNormalChance     | baseMajorChance     | baseEpicChance    | baseLegendaryChance  |
        |       |                      | (default: 25%)       | (default: 5%)       | (default: 0%)     | (default: 0%)        |
        | 10    | ~40% (calculated)    | ~35% (interpolated)  | ~20% (interpolated) | ~5% (interpolated)| ~0% (interpolated)   |
        | 20    | ~20% (calculated)    | ~30% (interpolated)  | ~30% (interpolated) | ~15% (interpolated)| ~5% (interpolated)  |
        | 30+   | Remainder (~10%)     | maxNormalChance      | maxMajorChance      | maxEpicChance     | maxLegendaryChance   |
        |       |                      | (default: 20%)       | (default: 35%)      | (default: 25%)    | (default: 10%)       |
        +-------+----------------------+----------------------+---------------------+-------------------+----------------------+
        
        The Minor quality is always calculated as the remainder after all other qualities are accounted for.
        The intermediate levels (10, 20) are interpolated based on the player's level progression.
        All distributions are controlled by the fields above and can be adjusted in the Inspector.
        */

        // Calculate chances based on level factor
        float legendaryChance = Mathf.Lerp(baseLegendaryChance, maxLegendaryChance, levelFactor);
        float epicChance = Mathf.Lerp(baseEpicChance, maxEpicChance, levelFactor);
        float majorChance = Mathf.Lerp(baseMajorChance, maxMajorChance, levelFactor);
        float normalChance = Mathf.Lerp(baseNormalChance, maxNormalChance, levelFactor);

        // Get a random value between 0 and 1
        float random = Random.value;

        // Calculate cumulative chances for each quality tier
        float legendaryThreshold = legendaryChance;
        float epicThreshold = legendaryThreshold + epicChance;
        float majorThreshold = epicThreshold + majorChance;
        float normalThreshold = majorThreshold + normalChance;
        // Minor is always the remainder (no threshold needed)

        // Return appropriate quality based on random value
        if (random < legendaryThreshold)
        {
            return Upgrade.UpgradeQuality.Legendary;
        }
        else if (random < epicThreshold)
        {
            return Upgrade.UpgradeQuality.Epic;
        }
        else if (random < majorThreshold)
        {
            return Upgrade.UpgradeQuality.Major;
        }
        else if (random < normalThreshold)
        {
            return Upgrade.UpgradeQuality.Normal;
        }
        else
        {
            return Upgrade.UpgradeQuality.Minor;
        }
    }
}