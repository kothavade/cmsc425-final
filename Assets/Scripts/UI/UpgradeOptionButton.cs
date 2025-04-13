using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UpgradeOptionButton : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Button button;

    private Upgrade upgrade;
    private Action<Upgrade> onClickCallback;

    public void Setup(Upgrade upgrade, Action<Upgrade> callback)
    {
        this.upgrade = upgrade;
        onClickCallback = callback;

        // Set UI elements
        if (iconImage != null) iconImage.sprite = upgrade.icon;
        if (titleText != null) titleText.text = upgrade.upgradeName;
        if (descriptionText != null) descriptionText.text = upgrade.description;

        // Set button click handler
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnButtonClick);
        Debug.Log("Added click listener to button: " + upgrade.upgradeName);
    }

    private void OnButtonClick()
    {
        Debug.Log("Button clicked: " + upgrade.upgradeName);
        onClickCallback?.Invoke(upgrade);
    }
}