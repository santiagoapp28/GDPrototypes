using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameConfig gameConfig;

    public Action<int> OnCoinsUpdated;
    public Action<int, int> OnEnergyUpdated;

    public int coins;
    public int wave;
    public int currentStageIndex = -1;
    public int health = 100;
    public int maxHealth = 100;
    public int maxHandCount;
    public int energy;
    public float timeScale = 1f;
    public List<int> energyCosts;
    private int energyCostIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes

        if (gameConfig == null)
        {
            Debug.LogError("GameConfig is not assigned in the GameManager. Please assign it in the inspector.", this);
            enabled = false;
            return;
        }

        GetConfigValues();

        if (currentStageIndex == -1 && FindAnyObjectByType<WaveManager>() != null)
            currentStageIndex = 0;
    }

    private void Update()
    {
        // Cheat codes have been moved to CheatManager.cs
    }

    public void ChangeTimeScale(float time)
    {
        timeScale = time;
        Time.timeScale = timeScale;
    }

    public void GetConfigValues()
    {
        coins = gameConfig.startingCoins;
        health = gameConfig.startingHealth;
        maxHealth = gameConfig.startingHealth;
        energy = gameConfig.startingEnergy;
        energyCosts = new List<int>(gameConfig.energyCostsPerDraw);
        maxHandCount = gameConfig.maxHandCount;
        energyCostIndex = 0;
    }

    public void NewStage()
    {
        health = maxHealth;
    }

    public void UpdateCoins(int addedCoins)
    {
        coins += addedCoins;
        OnCoinsUpdated?.Invoke(coins); // Notify subscribers
    }

    public void UpdateEnergy(int addedEnergy)
    {
        energy += addedEnergy;
        energy = Mathf.Clamp(energy, 0, CurrentMaxEnergy);

        OnEnergyUpdated?.Invoke(energy, CurrentMaxEnergy);
    }

    public int CurrentMaxEnergy => energyCosts.Count > 0 
        ? energyCosts[Mathf.Min(energyCostIndex, energyCosts.Count - 1)] 
        : 100;

    public void TryDrawCardsFromEnergy()
    {
        int currentCost = CurrentMaxEnergy;
        // Check if there is enough energy to draw.
        if (energy >= currentCost)
        {
            var deckUI = FindAnyObjectByType<DeckUI>();
            if (deckUI == null)
            {
                Debug.LogWarning("DeckUI not found in scene, cannot draw cards!");
                return;
            }

            // Assuming DeckUI has a public property `CurrentHandCount` that returns the number of cards in hand.
            int currentHandCount = deckUI.CurrentHandCount;
            int cardsToDraw = maxHandCount - currentHandCount;

            if (cardsToDraw <= 0)
            {
                Debug.Log("Hand is full. Cannot draw more cards.");
                return; // Do not consume energy if hand is full.
            }

            int amountToDraw = 3; // The number of cards to draw per energy bar fill.
            int actualCardsToDraw = Mathf.Min(amountToDraw, cardsToDraw);
            deckUI.DrawMultipleCards(actualCardsToDraw);

            energy -= currentCost;
            energyCostIndex++;
            OnEnergyUpdated?.Invoke(energy, CurrentMaxEnergy); // Notify UI of the change
        }
    }

    public void ResetEnergy()
    {
        energy = 0;
        energyCostIndex = 0;
        UpdateEnergy(0);
    }

    public void UpdateHealth(int addedHealth)
    {
        health += addedHealth;
        health = Mathf.Clamp(health, 0, maxHealth); // Ensure health doesn't exceed maxHealth
        FindAnyObjectByType<UIHealthManager>()?.UpdateHealth(health, maxHealth);

        if (health <= 0)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        FindAnyObjectByType<UIManager>().GameOverPanel();
        currentStageIndex = -1;
        GetConfigValues();
    }
}
