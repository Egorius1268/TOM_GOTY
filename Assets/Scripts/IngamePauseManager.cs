using UnityEngine;
using UnityEngine.SceneManagement;

public class InGamePauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    private bool isPaused = false;
    public static bool IsGamePaused;

    // Update is called once per frame
    public void Update()
    {
        IsGamePaused = isPaused;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }   
    }


    public void Resume()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Pause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Exit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
    
    
}
