using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    
    public TMP_Text moneyDisp;
    public TMP_Text hpDisp;
    public int startingPlayerHP = 100;
    public int PlayerHP;
    public int startMoney;
    public int moneyAmount = 0;

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
                    Debug.Log("Игра окончена");
                    // добавить логику завершения игры
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
}
