using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ModifierButton : MonoBehaviour
{
    [SerializeField] private ModifierTowerData modifierData; 
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private GameObject selectionIndicator;
    [SerializeField] private TextMeshProUGUI nameText;
    private int price;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnModifierSelected);
        Transform foundIndicator = transform.Find("SelectedBackground");
        if (foundIndicator != null)
        {
            selectionIndicator = foundIndicator.gameObject;
            selectionIndicator.SetActive(false);    
        }
        
        if (iconImage != null && modifierData != null && modifierData.icon != null)
            iconImage.sprite = modifierData.icon;
        if (modifierData != null)
        {
            price = modifierData.cost;
            priceText.text = price.ToString();
            
            if (nameText != null)
            {
                nameText.text = modifierData.name;
            }
        }
    }

    private void Update()
    {
        if (BuildManager.Instance == null) return;
        bool isSelected = BuildManager.Instance.GetSelectedModifier() == modifierData;
        
        if (selectionIndicator != null)
            selectionIndicator.SetActive(isSelected);
    }

    private void OnModifierSelected()
    {
        if (modifierData != null)
        {
            BuildManager.Instance.SelectModifier(modifierData);
        }
    }
}