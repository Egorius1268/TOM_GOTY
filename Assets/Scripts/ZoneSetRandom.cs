using UnityEngine;

public class ZoneSetRandomizer : MonoBehaviour
{
    [Header("Zone Sets List")]
    [SerializeField] private GameObject[] zoneSets;

    void Start()
    {
        if (zoneSets == null || zoneSets.Length == 0)
        {
            return;
        }
        int randomSetIndex = Random.Range(0, zoneSets.Length);
        for (int i = 0; i < zoneSets.Length; i++)
        {
            if (zoneSets[i] == null) continue;
            if (i == randomSetIndex)
            {
                zoneSets[i].SetActive(true);
            }
            else
            {
                zoneSets[i].SetActive(false);
            }
        }
    }
}
