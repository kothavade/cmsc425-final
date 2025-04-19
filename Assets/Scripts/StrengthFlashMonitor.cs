using UnityEngine;
using UnityEngine.UI;

public class StrengthFlashMonitor : MonoBehaviour
{
    [Header("References")]
    public PlayerStats playerStats;
    public Image strengthFlashImage;
    public float flashAlpha = 0.35f;
    public float pulseSpeed = 0.1f;
    public int pulseCount = 3;
    public float holdDuration = 0.5f;
    public float fadeDuration = 0.5f;

    private float previousStrength;
    private Coroutine flashRoutine;

    void Start()
    {
        if (playerStats == null || strengthFlashImage == null)
        {
            Debug.LogError("StrengthFlashMonitor: Missing references.");
            enabled = false;
            return;
        }

        strengthFlashImage.color = new Color(1f, 1f, 0f, 0f); 
        previousStrength = playerStats.strength;
    }

    void Update()
    {
        if (playerStats.strength > previousStrength)
        {
            TriggerStrengthPulse();
        }

        previousStrength = playerStats.strength;
    }

    void TriggerStrengthPulse()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(PulseStrengthEffect());
    }

    System.Collections.IEnumerator PulseStrengthEffect()
    {
        Color full = new Color(1f, 1f, 0f, flashAlpha);
        Color clear = new Color(1f, 1f, 0f, 0f);

        
        for (int i = 0; i < pulseCount; i++)
        {
            strengthFlashImage.color = full;
            yield return new WaitForSeconds(pulseSpeed);
            strengthFlashImage.color = clear;
            yield return new WaitForSeconds(pulseSpeed);
        }

        
        strengthFlashImage.color = full;
        yield return new WaitForSeconds(holdDuration);

       
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            strengthFlashImage.color = Color.Lerp(full, clear, elapsed / fadeDuration);
            yield return null;
        }

        strengthFlashImage.color = clear;
    }
}

