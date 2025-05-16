using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public List<Stage> stages;
    public int currentStageIndex = -1;
    private WaveManager _waveManager;
    public List<int> levelsScenesIDs = new List<int>();
    public int shopSceneID;

    public List<Wave> GetStageWaves()
    {
        return stages[Mathf.Clamp(currentStageIndex, 0, stages.Count - 1)].waves;
    }

    public void StartNewStage()
    {
        currentStageIndex++;
        if (currentStageIndex >= stages.Count)
        {
            Debug.Log("All stages completed!");
            return;
        }
        GoToNextLevel();
    }

    public void GoToNextLevel()
    {
        if (currentStageIndex < stages.Count)
        {
            SceneManager.LoadScene(levelsScenesIDs[currentStageIndex]);
            AudioManager.Instance.PlaySFX(Sounds.StartGame);
            AudioManager.Instance.PlayMusic(Music.GameplayMusic);
        }
        else
        {
            Debug.Log("All levels completed!");
        }
    }

    public void GoToShop()
    {
        AudioManager.Instance.PlaySFX(Sounds.UIClick);
        AudioManager.Instance.PlayMusic(Music.ShopMusic);
        SceneManager.LoadScene(shopSceneID);
    }
}
