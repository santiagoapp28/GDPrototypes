using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int coins;
    public int wave = 0;
    private UIManager uiManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes
    }

    private void Start()
    {
        uiManager = FindAnyObjectByType<UIManager>();
    }

    public void AddCoins(int addedCoins)
    {
        coins += addedCoins;
        uiManager.UpdateCoins(coins);
    }
}
