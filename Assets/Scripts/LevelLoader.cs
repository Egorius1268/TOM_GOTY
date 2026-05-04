using UnityEngine;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{

    public Button[] levelButtons;

    void Start()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        for (int i = 1; i < levelButtons.Length; i++)
        {
            levelButtons[i].interactable = (i + 1 <= unlockedLevel);
        }
    }
    
    

}
