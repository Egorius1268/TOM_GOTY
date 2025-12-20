using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.EventSystems; 

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;
    
    [Header("References")]
    public Tilemap buildableTilemap; 
    public LayerMask buildableLayer; 
    public LayerMask modifierBuildingLayer; 
    private GameManager gameManager;

    [Header("Turrets")]
    public TurretData[] turretPrefabs; // потом пригодится

    private Camera mainCam;
    private TurretData selectedTurret;
    private bool isInputBlocked = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        mainCam = Camera.main;
        DeselectTurret(); // начало без выбора
    }

    

    private void Update()
    {
        if (isInputBlocked) return;
        if (selectedTurret == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            TryBuildTurret();
        }
        else if (Input.GetMouseButtonDown(1)) // пкм отмена, потом мб на юи кнопку переташить
        {
            DeselectTurret();
        }
    }
    
    public void BlockInput(bool block)
    {
        isInputBlocked = block;

        if (block)
        {
            DeselectTurret(); 
            Debug.Log("BuildManager input blocked");
        }
    }
    public void SelectTurret(TurretData turret)
    {
        selectedTurret = turret;
      //  Debug.Log($"Selected: {turret.name}");
    }

    public void DeselectTurret()
    {
        selectedTurret = null; 
        //Debug.Log("Deselected turret");
    }

    private void TryBuildTurret()
    {
        if (!CanAffordTurret()) // проверка  на доступность покупки
        {
            return;
        }
        
        
        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f; // важно для 2D

        
        Vector3Int cellPos = buildableTilemap.WorldToCell(mouseWorldPos);

        if (IsOverModifierBuilding(mouseWorldPos))
        {
            Debug.Log("Cannot build on modifier buildings!");
            return;
        }
        if (!buildableTilemap.HasTile(cellPos))
        {
            Debug.Log("Cannot build here: not on buildable terrain.");
            return;
        }

        if (IsPointerOverUI()) 
        {
            return;
        }
        

        // проверка пересечения с уже построенным
        Collider2D occupied = Physics2D.OverlapCircle(mouseWorldPos, 0.4f, buildableLayer);
        if (occupied != null)
        {
            Debug.Log("Too close to another building!");
            return;
        }
        gameManager.DeductMoney(selectedTurret.cost);
        
        // строительство
        GameObject newObject = Instantiate(selectedTurret.prefab, mouseWorldPos, Quaternion.identity);
        
        Turret1 turretScript = newObject.GetComponent<Turret1>();
        
        if (turretScript != null)
        {
            turretScript.data = selectedTurret; // передаем данные с СО
            turretScript.InitializeFromData(); 
            Debug.Log($"Built turret: {selectedTurret.name}");
        }
        else
        {
            Debug.Log($"Built non-turret object: {selectedTurret.name}");
        }
        if (!gameManager.CanAfford(selectedTurret.cost))
        {
            Debug.Log($"Not enough money! Need {selectedTurret.cost}");
            return;
        }
        gameManager.DeductMoney(selectedTurret.cost); // вычет деняк
    }
    private bool CanAffordTurret()
    {
        return gameManager.CanAfford(selectedTurret.cost);
    }
        
    private bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject(); // указывает на ui а не на игровые объекты
    }
    private bool IsOverModifierBuilding(Vector3 position)
    {
        // Проверяем пересечение со слоем модификаторов
        Collider2D modifier = Physics2D.OverlapCircle(position, 0.3f, modifierBuildingLayer);
        return modifier != null;
    }
    
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || selectedTurret == null || buildableTilemap == null) return;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPos = buildableTilemap.WorldToCell(mouseWorldPos);
        Vector3 worldCenter = buildableTilemap.GetCellCenterWorld(cellPos);

        if (buildableTilemap.HasTile(cellPos))
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawWireCube(worldCenter, Vector3.one * 0.9f);
    }
}

