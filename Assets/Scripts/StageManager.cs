using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public List<Stage> stages;
    public int startingLevelIndex = 2;
    public int shopSceneID;

    public List<Wave> GetStageWaves()
    {
        return stages[Mathf.Clamp(GameManager.Instance.currentStageIndex, 0, stages.Count - 1)].waves;
    }

    public void StartNewStage()
    {
        GameManager.Instance.currentStageIndex++;
        if (GameManager.Instance.currentStageIndex >= stages.Count)
        {
            //WON THE GAME
            GameManager.Instance.currentStageIndex--;
            Debug.LogWarning("All stages completed. Repeating last stage");
        }
        GameManager.Instance.NewStage();
        GoToNextLevel();
    }

    public void GoToNextLevel()
    {
        if (GameManager.Instance.currentStageIndex < stages.Count)
        {
            SceneManager.LoadScene(startingLevelIndex + GameManager.Instance.currentStageIndex);
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

    public void GoToMenu()
    {
        AudioManager.Instance.PlaySFX(Sounds.UIClick);
        AudioManager.Instance.PlayMusic(Music.MenuMusic);
        SceneManager.LoadScene(0);
    }
}
