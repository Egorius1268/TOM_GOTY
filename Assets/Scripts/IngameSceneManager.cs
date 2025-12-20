using Unity.VisualScripting;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public class IngameSceneManager : MonoBehaviour
{
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
