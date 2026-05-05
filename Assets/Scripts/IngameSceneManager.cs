using Unity.VisualScripting;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public class IngameSceneManager : MonoBehaviour
{
    public GameObject levelsPanel;

    private void Awake()
    {
        if (levelsPanel != null)
        {
            levelsPanel.SetActive(false);
        }
    }

    public void SelectSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
        
    }
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void OpenLevelMenu()
    {
        levelsPanel.SetActive(true);
        
    }
    
    public void CloseLevelMenu()
    {
        levelsPanel.SetActive(false);
    }

    public void NewGame()
    {
        PlayerPrefs.SetInt("UnlockedLevel", 1); 
        PlayerPrefs.Save();
        SceneManager.LoadScene(1);
    }

    public void ContinueGame()
    {
        int lastUnlockedLevel = PlayerPrefs.GetInt("UnlockedLevel");
        Console.WriteLine("Last unlocked level: " + lastUnlockedLevel);
        if (lastUnlockedLevel == 0)
        {
            SceneManager.LoadScene(lastUnlockedLevel + 1);   
        }
        else
        {
            SceneManager.LoadScene(lastUnlockedLevel);    
        }
        
    }

    public void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
    public void QuitToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
