using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    
    public TMP_Text moneyDisp;
    public TMP_Text hpDisp;
    public GameObject deathPanel;
    public GameObject buildPanel;
    public GameObject hpAndCurrencyPanel;
    public GameObject levelCompletePanel;
    public static BuildManager Instance;
    public int startingPlayerHP = 100;
    public int PlayerHP;
    public int startMoney;
    public int moneyAmount = 0;
    public bool isGameOver = false;
    public bool isLevelComplete = false;

    private void Awake()
    {
        Time.timeScale = 1f;
    }

    private void Start()
    {
        AddMoney(startMoney);
        startHP();
        PlayerHP = 100;
    }

    public void AddMoney(int amnt)
    {
        moneyAmount += amnt;
        moneyDisp.text = "" + moneyAmount;
    }

    public void DeductMoney(int amnt)
    {
        moneyAmount -= amnt;
        moneyDisp.text = "" + moneyAmount;
    }
    private void startHP(){
        hpDisp.text = "" + startingPlayerHP;
    }
    public void DamageDealed(int damageAmount){
        if(PlayerHP > 0){
                PlayerHP -= damageAmount;
                UpdateHPDisplay();
                if (PlayerHP < 0)
                    PlayerHP = 0;
            
                if (PlayerHP <= 0)
                {
                    GameOver();
                }
        }
    }

    private void UpdateHPDisplay()
    {
        hpDisp.text = PlayerHP.ToString();
    }
    
    
    public bool CanAfford(int cost)
    {
        return moneyAmount >= cost;
    }

    private void GameOver()
    {
        isGameOver = true;
        
        
        Time.timeScale = 0f;

        
        if (deathPanel != null)
        {
            BuildManager.Instance.BlockInput(true);
            deathPanel.SetActive(true);
            buildPanel.SetActive(false);
            hpAndCurrencyPanel.SetActive(false);
            // StartCoroutine(ShowDeathPanel());
        } 
    }

    public void CompleteLevel()
    {
        if (isGameOver) return;
        isLevelComplete = true;
        Time.timeScale = 0f;
        if (buildPanel != null) buildPanel.SetActive(false);
        if (hpAndCurrencyPanel != null) hpAndCurrencyPanel.SetActive(false);

        // Показываем панель победы (должна быть назначена в инспекторе)
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
        }
    }
}
