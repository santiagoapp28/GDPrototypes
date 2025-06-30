using UnityEngine;

/// <summary>
/// Manages all cheat codes and debug functionality.
/// This component should only be included in development builds.
/// </summary>
public class CheatManager : MonoBehaviour
{
    void Update()
    {
        // Go to menu
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            var stageManager = FindAnyObjectByType<StageManager>();
            if (stageManager != null && GameManager.Instance != null)
            {
                stageManager.GoToMenu();
                GameManager.Instance.currentStageIndex = -1;
            }
        }

        // Finish stage and show relics
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            Cheat_FinishStageAndShowRelics();
        }

        // Max health
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.health = 9999;
                GameManager.Instance.maxHealth = 9999;
                FindAnyObjectByType<UIHealthManager>()?.UpdateHealth(GameManager.Instance.health, GameManager.Instance.maxHealth);
            }
        }

        // Max coins
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.coins = 9999;
                // Call with 0 to trigger the update event without adding more coins
                GameManager.Instance.UpdateCoins(0);
            }
        }

        // Max energy
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.energy = 9999;
                // Call with 0 to trigger the update event without adding more energy
                GameManager.Instance.UpdateEnergy(0);
            }
        }
    }

    private void Cheat_FinishStageAndShowRelics()
    {
        // Find the WaveManager to stop it and clean up its UI.
        var waveManager = FindAnyObjectByType<WaveManager>();
        if (waveManager != null)
        {
            // Disabling the component will stop its Update loop, coroutines,
            // and trigger OnDisable, which unsubscribes from enemy death events.
            waveManager.enabled = false;

            // Manually update UI elements controlled by WaveManager.
            if (waveManager.waveNameText != null)
                waveManager.waveNameText.text = "Stage Cleared!";
            if (waveManager.nextWaveButton != null)
                waveManager.nextWaveButton.gameObject.SetActive(false);
            if (waveManager.timeScaleButton != null)
                waveManager.timeScaleButton.gameObject.SetActive(false);
        }

        // Destroy all existing enemies.
        // Calling Die() ensures they grant rewards and play effects.
        var enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (var enemy in enemies)
        {
            enemy.Die();
        }

        // Show the relic selection panel.
        var uiManager = FindAnyObjectByType<UIManager>();
        if (uiManager != null)
        {
            uiManager.ShowRelicSelectionPanel();
            uiManager.ShowShopButton();
        }
    }
}