using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurretButton : MonoBehaviour
{
    [SerializeField] private TurretData turretData;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private GameObject selectionIndicator;
    [SerializeField] private TextMeshProUGUI nameText; 
    private int price;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnTurretSelected);
        Transform foundIndicator = transform.Find("SelectedBackground");
        if (foundIndicator != null)
        {
            selectionIndicator = foundIndicator.gameObject;
            selectionIndicator.SetActive(false);    
        }

        if (turretData != null)
        {
            if (iconImage != null && turretData.icon != null)
                iconImage.sprite = turretData.icon;
            price = turretData.cost;
            priceText.text = price.ToString();
            if (nameText != null)
            {
                nameText.text = turretData.name;
            }
        }
    }

    private void Update()
    {
        if (BuildManager.Instance == null) return;
        bool isSelected = BuildManager.Instance.GetSelectedBuilding() == turretData;
        selectionIndicator.SetActive(isSelected);
    }

    private void OnTurretSelected()
    {
        BuildManager.Instance.SelectTurret(turretData);
    }
}