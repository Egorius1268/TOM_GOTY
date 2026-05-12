using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurretButton : MonoBehaviour
{
    [SerializeField] private TurretData turretData;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI priceText;
    private int price;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnTurretSelected);

        if (iconImage != null && turretData.icon != null)
            iconImage.sprite = turretData.icon;

        price = turretData.cost;
        priceText.text = price.ToString();
    }

    private void OnTurretSelected()
    {
        BuildManager.Instance.SelectTurret(turretData);
    }
}