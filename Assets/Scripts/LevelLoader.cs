using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{

    public Button[] levelButtons;
    public Button continueButton;

    private void Awake()
    {
        int firstLaunch = PlayerPrefs.GetInt("IsNewGame", 1);
        
        if (firstLaunch == 1)
        {
            PlayerPrefs.SetInt("IsNewGame", 0);
            PlayerPrefs.SetInt("UnlockedLevel", 1);
        }
        
    }

    void Start()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 0);
        if (unlockedLevel > 1)
        {
            continueButton.interactable = true;
        }
        else
        {
            continueButton.interactable = false;
        }

        for (int i = 1; i < levelButtons.Length; i++)
        {
            levelButtons[i].interactable = (i + 1 <= unlockedLevel);
        }
    }
    
    
    

}
