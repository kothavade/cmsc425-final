using UnityEngine;
using UnityEngine.UI;

public class JumpFlashMonitor : MonoBehaviour
{
    [Header("References")]
    public PlayerStats playerStats;
    public Image jumpPulseImage;

    [Header("Effect Settings")]
    public float flashAlpha = 0.25f;
    public float pulseDuration = 0.4f;
    public float pulseScale = 1.3f;

    private float previousJumpForce;
    private Coroutine pulseRoutine;
    private RectTransform pulseRect;

    void Start()
    {
        if (playerStats == null || jumpPulseImage == null)
        {
            Debug.LogError("JumpFlashMonitor: Missing references.");
            enabled = false;
            return;
        }

        pulseRect = jumpPulseImage.rectTransform;
        jumpPulseImage.color = new Color(0f, 1f, 0f, 0f); 
        pulseRect.localScale = Vector3.one;
        previousJumpForce = playerStats.jumpForce;
    }

    void Update()
    {
        if (playerStats.jumpForce > previousJumpForce)
        {
            TriggerJumpPulse();
        }

        previousJumpForce = playerStats.jumpForce;
    }

    void TriggerJumpPulse()
    {
        if (pulseRoutine != null)
        {
            StopCoroutine(pulseRoutine);
        }

        pulseRoutine = StartCoroutine(PulseEffect());
    }

    System.Collections.IEnumerator PulseEffect()
    {
        Color startColor = new Color(0f, 1f, 0f, 0f);
        Color fullColor = new Color(0f, 1f, 0f, flashAlpha);
        Vector3 originalScale = Vector3.one;
        Vector3 targetScale = Vector3.one * pulseScale;

        float elapsed = 0f;
        while (elapsed < pulseDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / pulseDuration;

            
            jumpPulseImage.color = Color.Lerp(fullColor, startColor, t);
            pulseRect.localScale = Vector3.Lerp(targetScale, originalScale, t);

            yield return null;
        }

        jumpPulseImage.color = startColor;
        pulseRect.localScale = originalScale;
    }
}

