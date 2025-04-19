using UnityEngine;
using UnityEngine.UI;

public class DamageFlashMonitor : MonoBehaviour
{
    [Header("References")]
    public PlayerStats playerStats;      
    public Image damageFlashImage;       
    public float flashAlpha = 0.3f;      
    public float flashDuration = 0.2f;   

    private float previousHealth;
    private Coroutine flashRoutine;

    void Start()
    {
        if (playerStats == null)
        {
            Debug.LogError("DamageFlashMonitor: PlayerStats is not assigned!");
            enabled = false;
            return;
        }

        if (damageFlashImage == null)
        {
            Debug.LogError("DamageFlashMonitor: Damage flash Image is not assigned!");
            enabled = false;
            return;
        }

        damageFlashImage.color = new Color(1, 0, 0, 0); 
        previousHealth = playerStats.currentHealth;
    }

    void Update()
    {
        if (playerStats.currentHealth < previousHealth)
        {
            TriggerFlash();
        }

        previousHealth = playerStats.currentHealth;
    }

    void TriggerFlash()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }
        flashRoutine = StartCoroutine(Flash());
    }

    System.Collections.IEnumerator Flash()
    {
        damageFlashImage.color = new Color(1, 0, 0, flashAlpha);
        yield return new WaitForSeconds(flashDuration);
        damageFlashImage.color = new Color(1, 0, 0, 0);
    }
}
