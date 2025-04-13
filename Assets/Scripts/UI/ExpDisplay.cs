using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExpDisplay : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Image expBar;
    [SerializeField] private TextMeshProUGUI levelText;
    
    
    private void Update()
    {
        if (playerStats != null)
        {
            // Update exp bar
            expBar.fillAmount = playerStats.GetExpPercentage();
            levelText.text = playerStats.GetLevel().ToString();
            
        }
    }
}
