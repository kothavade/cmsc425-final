using UnityEngine;

[System.Serializable]
public class Upgrade
{
    public string upgradeName;
    public string description;
    public Sprite icon;
    public enum UpgradeType
    {
        MaxHealth,
        Speed,
        Strength,
        Defense,
        JumpForce,
        CommonPickupRange
    }
    public UpgradeType type;
    public float value;
}