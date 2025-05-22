using System;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameConfig gameConfig;

    public Action<int> OnCoinsUpdated;

    public int coins;
    public int wave;
    public int currentStageIndex = -1;
    public int health = 100;
    public int maxHealth = 100;
    public float timeScale = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes

        GetConfigValues();

        if (currentStageIndex == -1 && FindAnyObjectByType<WaveManager>() != null)
            currentStageIndex = 0;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Keypad0))
        {
            GetComponent<StageManager>().GoToMenu();
            currentStageIndex = -1;
        }
        if(Input.GetKeyDown(KeyCode.Keypad1))
        {
            GetComponent<StageManager>().GoToShop();
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            health = 9999;
            maxHealth = 9999;
            FindAnyObjectByType<UIManager>().UpdateHealth(health, maxHealth);
        }
        if(Input.GetKeyDown(KeyCode.Keypad3))
        {
            coins = 9999;
            UpdateCoins(coins);
        }
    }

    public void ChangeTimeScale(float time)
    {
        timeScale = time;
        Time.timeScale = timeScale;
    }

    private void GetConfigValues()
    {
        coins = gameConfig.startingCoins;
        health = gameConfig.startingHealth;
        maxHealth = gameConfig.startingHealth;
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

    public void UpdateHealth(int addedHealth)
    {
        health += addedHealth;
        health = Mathf.Clamp(health, 0, maxHealth); // Ensure health doesn't exceed maxHealth
        FindAnyObjectByType<UIManager>().UpdateHealth(health, maxHealth);

        if(health <= 0)
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
