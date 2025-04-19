using UnityEngine;
using UnityEngine.UI;

public class SpeedFlashMonitor : MonoBehaviour
{
    [Header("References")]
    public PlayerStats playerStats;
    public Image speedSwipeImage;

    [Header("Effect Settings")]
    public float swipeDuration = 0.4f;
    public float maxAlpha = 0.25f;
    public float swipeOffsetX = 1.2f; 

    private float previousSpeed;
    private Coroutine swipeRoutine;
    private RectTransform swipeRect;

    void Start()
    {
        if (playerStats == null || speedSwipeImage == null)
        {
            Debug.LogError("SpeedFlashMonitor: Missing references.");
            enabled = false;
            return;
        }

        swipeRect = speedSwipeImage.rectTransform;
        swipeRect.anchorMin = new Vector2(0.5f, 0.5f);
        swipeRect.anchorMax = new Vector2(0.5f, 0.5f);
        swipeRect.pivot = new Vector2(0.5f, 0.5f);

        speedSwipeImage.color = new Color(0f, 0.7f, 1f, 0f); 
        previousSpeed = playerStats.moveSpeed;
    }

    void Update()
    {
        if (playerStats.moveSpeed > previousSpeed)
        {
            TriggerSpeedSwipe();
        }

        previousSpeed = playerStats.moveSpeed;
    }

    void TriggerSpeedSwipe()
    {
        if (swipeRoutine != null)
        {
            StopCoroutine(swipeRoutine);
        }

        swipeRoutine = StartCoroutine(SwipeEffect());
    }

    System.Collections.IEnumerator SwipeEffect()
    {
        float elapsed = 0f;
        float screenWidth = Screen.width;
        Vector3 startPos = new Vector3(-screenWidth * swipeOffsetX, 0, 0);
        Vector3 endPos = new Vector3(screenWidth * swipeOffsetX, 0, 0);

        Color startColor = new Color(0f, 0.7f, 1f, 0f);
        Color fullColor = new Color(0f, 0.7f, 1f, maxAlpha);

        while (elapsed < swipeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / swipeDuration;

            
            swipeRect.anchoredPosition = Vector3.Lerp(startPos, endPos, t);

            
            float alpha = Mathf.Sin(t * Mathf.PI); 
            speedSwipeImage.color = new Color(0f, 0.7f, 1f, alpha * maxAlpha);

            yield return null;
        }

        speedSwipeImage.color = startColor;
        swipeRect.anchoredPosition = Vector3.zero;
    }
}


