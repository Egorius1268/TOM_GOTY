using UnityEngine;
using UnityEngine.UI;

public class TurretButton : MonoBehaviour
{
    [SerializeField] private TurretData turretData;
    [SerializeField] private Image iconImage;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnTurretSelected);

        if (iconImage != null && turretData.icon != null)
            iconImage.sprite = turretData.icon;
    }

    private void OnTurretSelected()
    {
        BuildManager.Instance.SelectTurret(turretData);
    }
}