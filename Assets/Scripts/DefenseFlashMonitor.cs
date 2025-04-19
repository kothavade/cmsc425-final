using UnityEngine;
using UnityEngine.UI;

public class DefenseFlashMonitor : MonoBehaviour
{
    [Header("References")]
    public PlayerStats playerStats;
    public Image defensePulseImage;

    [Header("Effect Settings")]
    public float pulseAlpha = 0.6f;
    public float singlePulseDuration = 0.6f;
    public int pulseCount = 2;

    private float previousDefense;
    private Coroutine pulseRoutine;

    void Start()
    {
        if (playerStats == null || defensePulseImage == null)
        {
            Debug.LogError("DefenseFlashMonitor: Missing references.");
            enabled = false;
            return;
        }

        defensePulseImage.color = new Color(0.4f, 0.25f, 0f, 0f); 
        previousDefense = playerStats.defense;
    }

    void Update()
    {
        if (playerStats.defense > previousDefense)
        {
            TriggerPulse();
        }

        previousDefense = playerStats.defense;
    }

    void TriggerPulse()
    {
        if (pulseRoutine != null)
        {
            StopCoroutine(pulseRoutine);
        }

        pulseRoutine = StartCoroutine(PulseSequence());
    }

    System.Collections.IEnumerator PulseSequence()
    {
        Color full = new Color(1f, 0.5f, 0f, pulseAlpha); 
        Color clear = new Color(1f, 0.5f, 0f, 0f);



        for (int i = 0; i < pulseCount; i++)
        {
            float elapsed = 0f;

          
            while (elapsed < singlePulseDuration / 2f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (singlePulseDuration / 2f);
                defensePulseImage.color = Color.Lerp(clear, full, t);
                yield return null;
            }

            elapsed = 0f;

            
            while (elapsed < singlePulseDuration / 2f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (singlePulseDuration / 2f);
                defensePulseImage.color = Color.Lerp(full, clear, t);
                yield return null;
            }
        }

      
        defensePulseImage.color = clear;
    }
}

