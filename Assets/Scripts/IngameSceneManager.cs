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
        PlayerPrefs.DeleteAll();
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}
