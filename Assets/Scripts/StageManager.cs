using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TowerDefense.Data;

public class StageManager : MonoBehaviour
{
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    [Header("Stages per Difficulty")]
    public List<Stage> easyStages;
    public List<Stage> mediumStages;
    public List<Stage> hardStages;

    [Header("Levels per Difficulty")]
    public List<LevelData> easyLevels;
    public List<LevelData> mediumLevels;
    public List<LevelData> hardLevels;

    [Header("Scene IDs")]
    public int gameplaySceneID = 2;
    public int shopSceneID;

    public List<Wave> GetStageWaves()
    {
        List<Stage> currentStages = GetCurrentStages();
        if (currentStages == null || currentStages.Count == 0)
        {
            Debug.LogError($"No stages defined for difficulty {GetDifficultyForCurrentStage()}", this);
            return new List<Wave>();
        }
        // Select a random stage from the list for the current difficulty
        int randomIndex = Random.Range(0, currentStages.Count);
        return currentStages[randomIndex].waves;
    }

    public LevelData GetCurrentLevelData()
    {
        List<LevelData> currentLevels = GetCurrentLevels();
        if (currentLevels == null || currentLevels.Count == 0)
        {
            Debug.LogError($"No level data defined for difficulty {GetDifficultyForCurrentStage()}", this);
            return null;
        }
        // Select a random level layout from the list for the current difficulty
        int randomIndex = Random.Range(0, currentLevels.Count);
        return currentLevels[randomIndex];
    }

    public void StartNewStage()
    {
        GameManager.Instance.ChangeTimeScale(1f); //reset timescale
        GameManager.Instance.currentStageIndex++;
        GameManager.Instance.NewStage();
        GoToNextLevel();
    }

    public void GoToNextLevel()
    {
        SceneManager.LoadScene(gameplaySceneID);
        AudioManager.Instance.PlaySFX(Sounds.StartGame);
        AudioManager.Instance.PlayMusic(Music.GameplayMusic);
    }

    public void GoToShop()
    {
        AudioManager.Instance.PlaySFX(Sounds.UIClick);
        AudioManager.Instance.PlayMusic(Music.ShopMusic);
        SceneManager.LoadScene(shopSceneID);
        GameManager.Instance.ChangeTimeScale(1f); //reset timescale
    }

    public void GoToMenu()
    {
        AudioManager.Instance.PlaySFX(Sounds.UIClick);
        AudioManager.Instance.PlayMusic(Music.MenuMusic);
        SceneManager.LoadScene(0);
        GameManager.Instance.ChangeTimeScale(1f); //reset timescale
    }

    private Difficulty GetDifficultyForCurrentStage()
    {
        int stage = GameManager.Instance.currentStageIndex;
        if (stage == 0) return Difficulty.Easy;
        if (stage == 1) return Difficulty.Medium;
        return Difficulty.Hard; // Stage 2 and above are Hard
    }

    private List<Stage> GetCurrentStages()
    {
        switch (GetDifficultyForCurrentStage())
        {
            case Difficulty.Easy: return easyStages;
            case Difficulty.Medium: return mediumStages;
            case Difficulty.Hard: return hardStages;
            default: return easyStages;
        }
    }

    private List<LevelData> GetCurrentLevels()
    {
        switch (GetDifficultyForCurrentStage())
        {
            case Difficulty.Easy: return easyLevels;
            case Difficulty.Medium: return mediumLevels;
            case Difficulty.Hard: return hardLevels;
            default: return easyLevels;
        }
    }
}
