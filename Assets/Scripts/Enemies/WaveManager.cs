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

    [Header("UI")]
    public TextMeshProUGUI waveNameText;
    public TextMeshProUGUI enemiesRemainingText;
    public Button nextWaveButton;
    public Button timeScaleButton;
    public float[] timeScaleOptions = { 1f, 2f, 4f };
    public TextMeshProUGUI timeScaleText;

    public int currentWaveIndex = -1;
    private SpawnState currentState;
    private Coroutine spawnWaveCoroutine;
    private List<Enemy> activeEnemies = new();
    private GridManager _gridManager;

    void Start()
    {
        StageManager stageManager = FindAnyObjectByType<StageManager>();
        _gridManager = FindAnyObjectByType<GridManager>();
        waves = stageManager.GetStageWaves();
        
        if (nextWaveButton != null)
        {
            nextWaveButton.onClick.AddListener(OnNextWaveButtonPressed);
            timeScaleButton.onClick.AddListener(OnTimeScaleButtonPressed);
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

    private int timeScaleIndex = 0;
    void SetState(SpawnState newState)
    {
        currentState = newState;

        if (nextWaveButton != null)
        {
            bool canPress = (currentState == SpawnState.WAITING_FOR_NEXT_WAVE) ||
                            (currentState == SpawnState.WAITING_TO_START && currentWaveIndex == -1 && waves.Count > 0);
            nextWaveButton.gameObject.SetActive(canPress);

            if (currentState == SpawnState.SPAWNING)
            {
                timeScaleButton.gameObject.SetActive(true);
            }
            else
            {
                timeScaleButton.gameObject.SetActive(false);
            }
        }
    }

    void OnTimeScaleButtonPressed()
    {
        if(timeScaleIndex >= timeScaleOptions.Length - 1)
        {
            timeScaleIndex = -1;
        }
        timeScaleIndex++;
        GameManager.Instance.ChangeTimeScale(timeScaleOptions[timeScaleIndex]);
        timeScaleText.text = "x" + timeScaleOptions[timeScaleIndex].ToString("0");
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
        if (_gridManager == null)
        {
            Debug.LogError("GridManager not found! Cannot determine spawn area.");
            return;
        }

        // Spawn within the last 3 rows (enemy area at the far end of the grid)
        int enemyAreaStartRow = _gridManager.GridHeight - 3;
        int spawnX = Random.Range(0, _gridManager.GridWidth);
        int spawnZ = Random.Range(enemyAreaStartRow, _gridManager.GridHeight);
        Vector2Int gridPos = new Vector2Int(spawnX, spawnZ);

        Vector3 spawnPos = _gridManager.GetSnappedWorldPosition(gridPos);
        // Enemies should face towards the castle (negative Z direction)
        GameObject enemyGO = Instantiate(enemyPrefab, spawnPos, Quaternion.LookRotation(Vector3.back));
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
                UpdateWaveNameText("Stage Cleared!");
                if (nextWaveButton != null) nextWaveButton.gameObject.SetActive(false);
                Debug.Log("STAGE COMPLETED!");

                FindAnyObjectByType<UIManager>().ShowRelicSelectionPanel();
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
        if (_gridManager == null || !Application.isPlaying) return;

        Gizmos.color = Color.red;

        // Define the corners of the spawn area in grid coordinates
        int enemyAreaStartRow = _gridManager.GridHeight - 3;
        Vector2Int bottomLeft = new Vector2Int(0, enemyAreaStartRow);
        Vector2Int topRight = new Vector2Int(_gridManager.GridWidth - 1, _gridManager.GridHeight - 1);
        Vector2Int topLeft = new Vector2Int(0, _gridManager.GridHeight - 1);
        Vector2Int bottomRight = new Vector2Int(_gridManager.GridWidth - 1, enemyAreaStartRow);

        // Get world positions for the corners
        Vector3 blPos = _gridManager.GetSnappedWorldPosition(bottomLeft);
        Vector3 trPos = _gridManager.GetSnappedWorldPosition(topRight);
        Vector3 tlPos = _gridManager.GetSnappedWorldPosition(topLeft);
        Vector3 brPos = _gridManager.GetSnappedWorldPosition(bottomRight);

        // Draw lines to form a rectangle
        Gizmos.DrawLine(blPos, brPos);
        Gizmos.DrawLine(brPos, trPos);
        Gizmos.DrawLine(trPos, tlPos);
        Gizmos.DrawLine(tlPos, blPos);
    }
}
