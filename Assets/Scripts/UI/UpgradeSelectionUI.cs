using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UpgradeSelectionUI : MonoBehaviour
{
    [Tooltip("Container for upgrade option buttons")]
    public Transform upgradeOptionsContainer;

    [Tooltip("Prefab for upgrade option buttons")]
    public GameObject upgradeOptionPrefab;

    private Action<Upgrade> onUpgradeSelectedCallback;

    private List<UpgradeOptionButton> buttonPool = new List<UpgradeOptionButton>();

    private void Start()
    {
        // Hide the panel initially
        gameObject.SetActive(true);
    }

    public void ShowUpgradeOptions(List<Upgrade> upgrades, Action<Upgrade> callback)
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        Debug.Log($"ShowUpgradeOptions called with {upgrades.Count} upgrades");
        // Print panel state
        Debug.Log($"Panel active state before: {gameObject.activeSelf}");

        if (callback == null)
        {
            Debug.LogError("Callback is null when showing upgrade options!");
        }
        onUpgradeSelectedCallback = callback;

        // Deactivate all pooled buttons
        foreach (var button in buttonPool)
        {
            button.gameObject.SetActive(false);
        }

        // Create or reuse option buttons
        for (int i = 0; i < upgrades.Count; i++)
        {
            UpgradeOptionButton optionButton;

            // Get or create button
            if (i < buttonPool.Count)
            {
                optionButton = buttonPool[i];
                optionButton.gameObject.SetActive(true);
            }
            else
            {
                GameObject optionObj = Instantiate(upgradeOptionPrefab, upgradeOptionsContainer);
                optionButton = optionObj.GetComponent<UpgradeOptionButton>();
                buttonPool.Add(optionButton);
            }

            // Set up the button
            Debug.Log($"Setting up button {upgrades}");
            optionButton.Setup(upgrades[i], OnUpgradeButtonClicked);
        }

        // Show the panel
        gameObject.SetActive(true);
        Debug.Log($"Panel active state after: {gameObject.activeSelf}");
    }

    private void OnUpgradeButtonClicked(Upgrade upgrade)
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // Hide the panel
        gameObject.SetActive(false);

        // Call the callback
        onUpgradeSelectedCallback?.Invoke(upgrade);
    }
}