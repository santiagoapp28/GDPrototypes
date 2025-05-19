using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameConfig gameConfig;

    public int coins;
    public int wave = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes

        coins = gameConfig.startingCoins;
    }

    public void UpdateCoins(int addedCoins)
    {
        coins += addedCoins;
        FindAnyObjectByType<UIManager>().UpdateCoins(coins);
    }
}
