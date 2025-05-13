// WaveSpawner.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; // For basic UI feedback (optional)
using TMPro; // If using TextMeshPro for UI

public class WaveManager : MonoBehaviour
{
    public enum SpawnState { WAITING_TO_START, SPAWNING, WAITING_FOR_NEXT_WAVE, ALL_WAVES_COMPLETE }

    [Header("Wave Configuration")]
    public List<Wave> waves; // Assign your Wave ScriptableObjects here in the Inspector
    public Transform[] spawnPoints; // Points where enemies will be instantiated

    [Header("Timing")]
    public float initialDelayBeforeFirstWave = 5f; // Time before the very first wave starts

    [Header("UI (Optional)")]
    public TextMeshProUGUI waveNameText;
    public TextMeshProUGUI waveCountdownText;
    public TextMeshProUGUI enemiesRemainingText;
    public Button nextWaveButton; // Button to manually start next wave

    private int currentWaveIndex = -1; // -1 indicates game hasn't started or waves haven't begun
    private SpawnState currentState;
    private Coroutine waveCountdownCoroutine;
    private Coroutine spawnWaveCoroutine;

    private List<Enemy> activeEnemies = new List<Enemy>();

    void Start()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("WaveSpawner: No spawn points assigned!");
            enabled = false;
            return;
        }

        if (nextWaveButton != null)
        {
            nextWaveButton.onClick.AddListener(OnNextWaveButtonPressed);
            nextWaveButton.gameObject.SetActive(false); // Hide initially
        }

        SetState(SpawnState.WAITING_TO_START);
        StartCoroutine(InitialGameDelay());
    }

    void OnEnable()
    {
        Enemy.OnEnemyDied += HandleEnemyDeath;
    }

    void OnDisable()
    {
        Enemy.OnEnemyDied -= HandleEnemyDeath;
    }

    IEnumerator InitialGameDelay()
    {
        UpdateWaveNameText("Game Starting Soon...");
        UpdateWaveCountdownText(initialDelayBeforeFirstWave);
        yield return new WaitForSeconds(initialDelayBeforeFirstWave);
        PrepareNextWave();
    }

    void Update()
    {
        // You could put some checks here, but most logic is event or coroutine driven
        if (enemiesRemainingText != null)
        {
            enemiesRemainingText.text = "Enemies: " + activeEnemies.Count;
        }
    }

    void SetState(SpawnState newState)
    {
        currentState = newState;
        // Debug.Log("Spawner new state: " + newState);

        if (nextWaveButton != null)
        {
            // Show "Next Wave" button only when waiting for player input after a wave is cleared or before first wave
            bool canManuallyStart = (currentState == SpawnState.WAITING_FOR_NEXT_WAVE) ||
                                    (currentState == SpawnState.WAITING_TO_START && currentWaveIndex == -1 && waves.Count > 0);
            nextWaveButton.gameObject.SetActive(canManuallyStart);
        }
    }

    void PrepareNextWave()
    {
        currentWaveIndex++;

        if (currentWaveIndex >= waves.Count)
        {
            SetState(SpawnState.ALL_WAVES_COMPLETE);
            UpdateWaveNameText("All Waves Cleared!");
            UpdateWaveCountdownText(0);
            if (nextWaveButton != null) nextWaveButton.gameObject.SetActive(false);
            Debug.Log("ALL WAVES COMPLETED!");
            // Potentially trigger game win condition here
            return;
        }

        Wave currentWaveAsset = waves[currentWaveIndex];
        UpdateWaveNameText(!string.IsNullOrEmpty(currentWaveAsset.waveName) ? currentWaveAsset.waveName : "Wave " + (currentWaveIndex + 1));
        SetState(SpawnState.WAITING_FOR_NEXT_WAVE);

        if (waveCountdownCoroutine != null) StopCoroutine(waveCountdownCoroutine);
        waveCountdownCoroutine = StartCoroutine(WaveCountdown(currentWaveAsset.timeToNextWave));
    }

    IEnumerator WaveCountdown(float duration)
    {
        float timer = duration;
        while (timer > 0)
        {
            UpdateWaveCountdownText(timer);
            yield return new WaitForSeconds(1f); // Update UI every second
            timer--;
        }
        UpdateWaveCountdownText(0);
        StartCurrentWave();
    }

    void OnNextWaveButtonPressed()
    {
        if (currentState == SpawnState.WAITING_FOR_NEXT_WAVE || (currentState == SpawnState.WAITING_TO_START && currentWaveIndex == -1))
        {
            if (waveCountdownCoroutine != null)
            {
                StopCoroutine(waveCountdownCoroutine);
                waveCountdownCoroutine = null;
            }
            // Optionally give bonus for starting early
            StartCurrentWave();
        }
    }

    void StartCurrentWave()
    {
        if (currentWaveIndex < 0 || currentWaveIndex >= waves.Count)
        {
            Debug.LogError("Tried to start wave with invalid index: " + currentWaveIndex);
            if (currentWaveIndex >= waves.Count && waves.Count > 0) // If we went past last wave, try to prepare next (which will lead to ALL_WAVES_COMPLETE)
            {
                PrepareNextWave();
            }
            return;
        }

        SetState(SpawnState.SPAWNING);
        if (nextWaveButton != null) nextWaveButton.gameObject.SetActive(false); // Hide during spawn
        UpdateWaveCountdownText(0); // Clear countdown

        Wave waveToSpawn = waves[currentWaveIndex];
        if (spawnWaveCoroutine != null) StopCoroutine(spawnWaveCoroutine);
        spawnWaveCoroutine = StartCoroutine(SpawnWave(waveToSpawn));
    }

    IEnumerator SpawnWave(Wave wave)
    {
        // Debug.Log("Spawning Wave: " + (currentWaveIndex + 1));
        activeEnemies.Clear(); // Clear list for the new wave

        foreach (EnemyGroup group in wave.enemyGroups)
        {
            if (group.startDelay > 0)
            {
                yield return new WaitForSeconds(group.startDelay);
            }

            for (int i = 0; i < group.count; i++)
            {
                if (group.enemyPrefab == null)
                {
                    Debug.LogError($"Enemy prefab in wave '{wave.waveName}' group is null. Skipping this enemy.");
                    continue;
                }
                SpawnEnemy(group.enemyPrefab);
                if (i < group.count - 1) // Don't wait after the last enemy in the group
                {
                    yield return new WaitForSeconds(group.spawnInterval);
                }
            }
        }
        // All enemies for this wave have been dispatched for spawning.
        // The state remains SPAWNING until all activeEnemies are handled (defeated or reached end).
        // CheckIfWaveClear will handle moving to next state.
        yield return null; // Ensure coroutine finishes this frame
        CheckIfWaveClear(); // Initial check in case wave was empty or all enemies died instantly
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        if (spawnPoints.Length == 0) return; // Should have been caught in Start, but safety check

        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemyGO = Instantiate(enemyPrefab, randomSpawnPoint.position, randomSpawnPoint.rotation);
        Enemy newEnemy = enemyGO.GetComponent<Enemy>();

        if (newEnemy != null)
        {
            activeEnemies.Add(newEnemy);
        }
        else
        {
            Debug.LogError("Spawned object does not have an Enemy component: " + enemyPrefab.name);
            Destroy(enemyGO); // Clean up invalid spawn
        }
    }

    void HandleEnemyDeath(Enemy deadEnemy)
    {
        if (activeEnemies.Contains(deadEnemy))
        {
            activeEnemies.Remove(deadEnemy);
        }

        CheckIfWaveClear();
    }

    void CheckIfWaveClear()
    {
        // Only proceed if the wave was actually spawning and now all dispatched enemies are gone
        if (currentState == SpawnState.SPAWNING && activeEnemies.Count == 0)
        {
            // Check if the spawnWaveCoroutine has finished dispatching all enemies
            // This is a bit tricky as the coroutine might finish, but enemies are still alive.
            // The primary condition is activeEnemies.Count == 0 after spawning is complete.

            // We need to ensure that the SpawnWave coroutine itself has finished its iteration.
            // A simple way is to rely on activeEnemies.Count. If it's 0, and we *were* spawning,
            // it implies all spawned units are now dealt with.

            Debug.Log("Wave " + (currentWaveIndex + 1) + " cleared!");
            // Stop the spawning coroutine if it's somehow still lingering (shouldn't be if logic is right)
            if (spawnWaveCoroutine != null)
            {
                StopCoroutine(spawnWaveCoroutine);
                spawnWaveCoroutine = null;
            }
            PrepareNextWave();
        }
    }


    // --- UI Update Methods (Basic) ---
    void UpdateWaveNameText(string text)
    {
        if (waveNameText != null) waveNameText.text = text;
    }

    void UpdateWaveCountdownText(float time)
    {
        if (waveCountdownText != null)
        {
            if (time > 0) waveCountdownText.text = "Next Wave: " + Mathf.CeilToInt(time).ToString();
            else waveCountdownText.text = "";
        }
    }
}