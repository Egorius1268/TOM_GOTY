using UnityEngine;

public class BuildingsTabsManager : MonoBehaviour
{
    public GameObject[] panels;

    public void OpenTab(int tabIndex)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }

        if (tabIndex >= 0 && tabIndex < panels.Length)
        {
            panels[tabIndex].SetActive(true);    
        }
        
    }
}
