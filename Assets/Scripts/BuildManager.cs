using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    [Header("References")] public Tilemap buildableTilemap;
    public LayerMask buildableLayer;
    public LayerMask modifierBuildingLayer;
    public LayerMask trapBuildingLayer;
    private GameManager gameManager;

    [Header("Turrets")] public TurretData[] turretPrefabs; // потом пригодится

    private Camera mainCam;
    private TurretData selectedTurret;
    private ModifierTowerData selectedModifier;
    private bool isBuildingModifier = false;
    private GameObject ghostSpriteObject;
    private SpriteRenderer ghostSpriteRenderer;
    private GameObject spawnedRangeIndicator; // Ссылка на созданный под призраком круг
    private SpriteRenderer indicatorRenderer;  
    private bool isInputBlocked = false;
    public bool isTrap;
    private bool isSellMode = false;

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


        ghostSpriteObject = new GameObject("ghostSprite");
        ghostSpriteRenderer = ghostSpriteObject.AddComponent<SpriteRenderer>();

        ghostSpriteRenderer.color = new Color(1f, 1f, 1f, 0.3f);
        ghostSpriteRenderer.sortingOrder = 50;

        ghostSpriteObject.SetActive(false);

        DeselectTurret(); // начало без выбора
    }



    private void Update()
    {
        if (isInputBlocked) return;
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUI()) return;

            if (isSellMode)
            {
                TrySellTurret();
                return;
            }

            if (InGamePauseManager.IsGamePaused) return;

            if (selectedTurret != null || selectedModifier != null)
            {
                TryBuildTurret();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            DeselectTurret();
            return;
        }

        if (selectedTurret == null && selectedModifier == null) return;
        if (ghostSpriteObject.activeSelf)
        {
            Vector3 mainCamPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            mainCamPos.z = 0;
            ghostSpriteObject.transform.position = mainCamPos;
        }


    


        // else if (Input.GetMouseButtonDown(1)) // пкм отмена, потом мб на юи кнопку переташить
        // {
        //     DeselectTurret();
        // }
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
        DeselectTurret();
        isSellMode = false;
        selectedTurret = turret;

        if (ghostSpriteRenderer != null && selectedTurret != null)
        {
            ghostSpriteRenderer.sprite = selectedTurret.worldSprite;
            Color ghostColor = selectedTurret.towerColor;
            ghostColor.a = 0.3f;
            ghostSpriteRenderer.color = ghostColor; 
            
            ghostSpriteObject.SetActive(true);
            SetupRangeIndicator(selectedTurret.rangeCircleSprite, selectedTurret.range);
        }
        
         //  Debug.Log($"Selected: {turret.name}");
        SetZonesVision(true); 
    }
    
    public void SelectModifier(ModifierTowerData modifier)
    {
        DeselectTurret();
        isSellMode = false;

        selectedModifier = modifier;
        isBuildingModifier = true;

        if (ghostSpriteRenderer != null && selectedModifier != null)
        {
            Color ghostColor = selectedModifier.towerColor;
            ghostColor.a = 0.3f; 
            ghostSpriteRenderer.sprite = selectedModifier.worldSprite; 
            ghostSpriteRenderer.color = selectedModifier.towerColor; 
            
            ghostSpriteObject.SetActive(true);
            SetupRangeIndicator(selectedModifier.rangeCircleSprite, selectedModifier.triggerRadius);
        }
        
        SetZonesVision(true);
    }

    public void DeselectTurret()
    {
        selectedTurret = null; 
        selectedModifier = null;
        isBuildingModifier = false;
        if (ghostSpriteRenderer != null)
        {
            ghostSpriteRenderer.sprite = null;
            ghostSpriteRenderer.color = new Color(1f, 1f, 1f, 0.3f); 
        }
        if (spawnedRangeIndicator != null) spawnedRangeIndicator.SetActive(false);
        
        if (ghostSpriteObject != null) ghostSpriteObject.SetActive(false);   
        
        //Debug.Log("Deselected turret");
        SetZonesVision(false);
    }

    private void TryBuildTurret()
    {
        int currentCost = isBuildingModifier ? selectedModifier.cost : selectedTurret.cost;
        GameObject prefabToSpawn = isBuildingModifier ? selectedModifier.prefab : selectedTurret.prefab;
        
        if (prefabToSpawn == null) return;
        
        if (!gameManager.CanAfford(currentCost)) // проверка  на доступность покупки
        {
            return;
        }
        
        
        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f; // важно для 2D

        
        Vector3Int cellPos = buildableTilemap.WorldToCell(mouseWorldPos);

        if (selectedTurret != null && !isBuildingModifier)
        {
            if (selectedTurret.buildingType == BuildingType.Trap || selectedTurret.buildingType == BuildingType.Barrier)
            {
                Collider2D road = Physics2D.OverlapPoint(mouseWorldPos, trapBuildingLayer);
                if (road == null)
                {
                    Debug.Log("поптыка стройки ловушки вне дороги");
                    return;
                }
            }
            else
            {
                if (!buildableTilemap.HasTile(cellPos))
                {
                    return;
                }
            }
            
        }else if (isBuildingModifier)
        {   
            if (!buildableTilemap.HasTile(cellPos)) return;
        }

        if (IsOverModifierBuilding(mouseWorldPos))
        {
            Debug.Log("cant build on modifier buildings");
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
        gameManager.DeductMoney(currentCost);
        
        // строительство
        GameObject newObject = Instantiate(prefabToSpawn, mouseWorldPos, Quaternion.identity);
        
        if (isBuildingModifier)
        {
            BulletModifier modifierScript = newObject.GetComponent<BulletModifier>();
            if (modifierScript != null)
            {
                modifierScript.data = selectedModifier;
                modifierScript.InitializeFromData();
            }
        }
        else
        {
            TrapTrigger trapScript = newObject.GetComponent<TrapTrigger>();
            if (trapScript != null)
            {
                trapScript.data = selectedTurret;
                trapScript.InitializeFromData();
            }else if (newObject.TryGetComponent<BarrierWall>(out BarrierWall barrierScript))
            {
                barrierScript.data = selectedTurret;
                barrierScript.InitializeFromData();
            }
            else
            {
                Turret1 turretScript = newObject.GetComponent<Turret1>();
        
                if (turretScript != null)
                {
                    turretScript.data = selectedTurret; // передаем данные с СО
                    turretScript.InitializeFromData(); 
                    Debug.Log($"Built turret: {selectedTurret.name}");
                }
            }
        }
        
        
        
        if (!gameManager.CanAfford(currentCost))
        {
            DeselectTurret();   
        }
        //gameManager.DeductMoney(selectedTurret.cost); // вычет деняк
    }
    
    private void TrySellTurret()
{
    Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
    mouseWorldPos.z = 0f;
    Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, buildableLayer);

    if (hit != null)
    {
        Turret1 turret = hit.GetComponent<Turret1>();
        if (turret != null && turret.data != null)
        {
            int refund = Mathf.RoundToInt(turret.data.cost * 0.5f); 
            if (GameManager.Instance != null) GameManager.Instance.AddMoney(refund);
            Destroy(hit.gameObject);
            return;
        }
        BarrierWall barrier = hit.GetComponent<BarrierWall>();
        if (barrier != null && barrier.data != null)
        {
            int refund = Mathf.RoundToInt(barrier.data.cost * 0.5f);
            if (GameManager.Instance != null) GameManager.Instance.AddMoney(refund);
            Destroy(hit.gameObject);
            return;
        }
        BulletModifier modifier = hit.GetComponent<BulletModifier>();
        if (modifier != null && modifier.data != null)
        {
            int refund = Mathf.RoundToInt(modifier.data.cost * 0.5f);
            if (GameManager.Instance != null) GameManager.Instance.AddMoney(refund);
            Destroy(hit.gameObject);
            return;
        }
        
        TrapTrigger trap = hit.GetComponent<TrapTrigger>();
        if (trap != null && trap.data != null)
        {
            int refund = Mathf.RoundToInt(trap.data.cost * 0.5f);
            if (GameManager.Instance != null) GameManager.Instance.AddMoney(refund);
            Destroy(hit.gameObject);
            return;
        }
    }
}
    public void ToggleSellMode()
    {
        isSellMode = !isSellMode;
        if (isSellMode)
        {
            DeselectTurret();
        }
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
        Collider2D modifier = Physics2D.OverlapCircle(position, 0.3f, modifierBuildingLayer);
        return modifier != null;
    }
    
    private bool IsOverTrap(Vector3 position)
    {
        Collider2D trap = Physics2D.OverlapCircle(position, 0.3f, trapBuildingLayer);
        return trap != null;
    }
    
    
    private void SetZonesVision(bool visible)
    {
        Buff_DebuffZones[] zones = Object.FindObjectsByType<Buff_DebuffZones>(FindObjectsSortMode.None);
    
        foreach (var zone in zones)
        {
            SpriteRenderer sr = zone.GetComponent<SpriteRenderer>();
            if (sr != null) 
            {
                sr.enabled = visible;
            }
        }
    }

    private void SetupRangeIndicator(Sprite circleSprite, float rangeValue)
    {
        if (ghostSpriteObject == null || circleSprite == null) return;
        if (spawnedRangeIndicator == null)
        {
            spawnedRangeIndicator = new GameObject("GhostRangeIndicator");
            spawnedRangeIndicator.transform.SetParent(ghostSpriteObject.transform);
            spawnedRangeIndicator.transform.localPosition = Vector3.zero;
            indicatorRenderer = spawnedRangeIndicator.AddComponent<SpriteRenderer>();
        }
        indicatorRenderer.sprite = circleSprite;
        
        Color indicatorColor = Color.white;
        indicatorColor.a = 0.3f; 
        indicatorRenderer.color = indicatorColor;
        
        indicatorRenderer.sortingLayerName = ghostSpriteRenderer.sortingLayerName;
        indicatorRenderer.sortingOrder = ghostSpriteRenderer.sortingOrder - 1;
        spawnedRangeIndicator.SetActive(true);
        float diameter = rangeValue * 2f;
        spawnedRangeIndicator.transform.localScale = new Vector3(diameter, diameter, 1f);
    }

    public TurretData GetSelectedBuilding()
    {
        return selectedTurret;
    }
    
    public ModifierTowerData GetSelectedModifier()
    {
        return selectedModifier;
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

