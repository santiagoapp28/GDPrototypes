using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class WaveManager : MonoBehaviour
{
    public enum SpawnState { WAITING_TO_START, SPAWNING, WAITING_FOR_NEXT_WAVE, ALL_WAVES_COMPLETE }

    [Header("Wave Configuration")]
    public List<Wave> waves;
    public Transform minSpawnPos;
    public Transform maxSpawnPos;

    [Header("UI")]
    public TextMeshProUGUI waveNameText;
    public TextMeshProUGUI enemiesRemainingText;
    public Button nextWaveButton;

    public int currentWaveIndex = -1;
    private SpawnState currentState;
    private Coroutine spawnWaveCoroutine;
    private List<Enemy> activeEnemies = new();

    void Start()
    {
        waves = FindAnyObjectByType<StageManager>().GetStageWaves();

        if (nextWaveButton != null)
        {
            nextWaveButton.onClick.AddListener(OnNextWaveButtonPressed);
            nextWaveButton.gameObject.SetActive(true); // Show at start
        }

        SetState(SpawnState.WAITING_TO_START);
    }

    void OnEnable()
    {
        Enemy.OnEnemyDied += HandleEnemyDeath;
    }

    void OnDisable()
    {
        Enemy.OnEnemyDied -= HandleEnemyDeath;
    }

    void Update()
    {
        if (enemiesRemainingText != null)
        {
            enemiesRemainingText.text = "Enemies: " + activeEnemies.Count;
        }
    }

    void SetState(SpawnState newState)
    {
        currentState = newState;

        if (nextWaveButton != null)
        {
            bool canPress = (currentState == SpawnState.WAITING_FOR_NEXT_WAVE) ||
                            (currentState == SpawnState.WAITING_TO_START && currentWaveIndex == -1 && waves.Count > 0);
            nextWaveButton.gameObject.SetActive(canPress);
        }
    }

    void OnNextWaveButtonPressed()
    {
        if (currentState == SpawnState.WAITING_FOR_NEXT_WAVE || (currentState == SpawnState.WAITING_TO_START && currentWaveIndex == -1))
        {
            StartCurrentWave();
        }
    }

    void StartCurrentWave()
    {
        currentWaveIndex++;

        Wave wave = waves[currentWaveIndex];
        UpdateWaveNameText(!string.IsNullOrEmpty(wave.waveName) ? wave.waveName : "Wave " + (currentWaveIndex + 1));

        SetState(SpawnState.SPAWNING);
        if (nextWaveButton != null) nextWaveButton.gameObject.SetActive(false);

        if (spawnWaveCoroutine != null) StopCoroutine(spawnWaveCoroutine);
        spawnWaveCoroutine = StartCoroutine(SpawnWave(wave));
    }

    public void GoToShop()
    {
        GameManager.Instance.GetComponent<StageManager>().GoToShop();
    }

    IEnumerator SpawnWave(Wave wave)
    {
        activeEnemies.Clear();

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
                    Debug.LogError($"Enemy prefab in wave '{wave.waveName}' group is null. Skipping.");
                    continue;
                }

                SpawnEnemy(group.enemyPrefab);
                if (i < group.count - 1)
                    yield return new WaitForSeconds(group.spawnInterval);
            }
        }

        yield return null;
        CheckIfWaveClear();
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        Vector3 spawnPos = new Vector3(Random.Range(minSpawnPos.transform.position.x, maxSpawnPos.transform.position.x),
                                       minSpawnPos.transform.position.y,
                                       Random.Range(minSpawnPos.transform.position.z, maxSpawnPos.transform.position.z));
        GameObject enemyGO = Instantiate(enemyPrefab, spawnPos, maxSpawnPos.rotation);
        Enemy newEnemy = enemyGO.GetComponent<Enemy>();

        if (newEnemy != null)
        {
            activeEnemies.Add(newEnemy);
        }
        else
        {
            Debug.LogError("Spawned object missing Enemy component: " + enemyPrefab.name);
            Destroy(enemyGO);
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
        if (currentState == SpawnState.SPAWNING && activeEnemies.Count == 0)
        {
            if (spawnWaveCoroutine != null)
            {
                StopCoroutine(spawnWaveCoroutine);
                spawnWaveCoroutine = null;
            }

            Debug.Log("Wave " + (currentWaveIndex + 1) + " cleared!");
            SetState(SpawnState.WAITING_FOR_NEXT_WAVE);

            AudioManager.Instance.PlaySFX(Sounds.WaveCleared);

            if (currentWaveIndex + 1 >= waves.Count)
            {
                SetState(SpawnState.ALL_WAVES_COMPLETE);
                UpdateWaveNameText("All Waves Cleared!");
                if (nextWaveButton != null) nextWaveButton.gameObject.SetActive(false);
                Debug.Log("ALL WAVES COMPLETED!");

                FindAnyObjectByType<UIManager>().ShowShopButton();
                return;
            }
        }
    }

    void UpdateWaveNameText(string text)
    {
        if (waveNameText != null) waveNameText.text = text;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        float minposX = minSpawnPos.position.x;
        float minposZ = minSpawnPos.position.z;
        float maxposX = maxSpawnPos.position.x;
        float maxposZ = maxSpawnPos.position.z;
        Gizmos.DrawSphere(new Vector3(minposX, minSpawnPos.position.y, minposZ), 0.2f);
        Gizmos.DrawSphere(new Vector3(maxposX, maxSpawnPos.position.y, maxposZ), 0.2f);
        Gizmos.DrawSphere(new Vector3(minposX, maxSpawnPos.position.y, maxposZ), 0.2f);
        Gizmos.DrawSphere(new Vector3(maxposX, maxSpawnPos.position.y, minposZ), 0.2f);

    }
}
