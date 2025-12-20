using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class WaypointGenerator : MonoBehaviour
{
    public static WaypointGenerator Instance; // Singleton для удобства доступа

    [Header("Настройки")]
    public TileBase roadTile;
    public GameObject waypointPrefab; // можно использовать только для визуализации
    public GameObject startWaypointPrefab;
    public GameObject endWaypointPrefab;

    private Tilemap tilemap;
    public List<Vector3> Path { get; private set; } = new List<Vector3>(); // 🔑 Главное — путь в правильном порядке!

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        tilemap = GetComponent<Tilemap>();
        GeneratePath();
    }

    private void GeneratePath()
    {
        Path.Clear();

        // Найдём любой стартовый тайл (первый попавшийся тайл дороги)
        Vector3Int? start = null;
        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos) && tilemap.GetTile(pos) == roadTile)
            {
                start = pos;
                break;
            }
        }

        if (start == null)
        {
            Debug.LogError("No road tile found!");
            return;
        }

        // Идём по дороге
        TraversePath(start.Value);

        // Визуализация (опционально)
        VisualizePath();
    }

    private void TraversePath(Vector3Int current)
    {
        Vector3Int[] directions = {
            Vector3Int.up, Vector3Int.down,
            Vector3Int.left, Vector3Int.right
        };

        Path.Add(tilemap.GetCellCenterWorld(current));

        while (true)
        {
            Vector3Int next = current;
            bool foundNext = false;

            foreach (var dir in directions)
            {
                Vector3Int candidate = current + dir;
                if (tilemap.HasTile(candidate) && tilemap.GetTile(candidate) == roadTile)
                {
                    // Проверим, не был ли этот тайл уже добавлен (чтобы не идти назад)
                    Vector3 worldCandidate = tilemap.GetCellCenterWorld(candidate);
                    if (!Path.Contains(worldCandidate))
                    {
                        next = candidate;
                        foundNext = true;
                        break; // идём в первом найденном направлении
                    }
                }
            }

            if (!foundNext) break;

            current = next;
            Path.Add(tilemap.GetCellCenterWorld(current));
        }
    }

    private void VisualizePath()
    {
        if (waypointPrefab == null) return;

        for (int i = 0; i < Path.Count; i++)
        {
            GameObject prefab = waypointPrefab;
            if (i == 0 && startWaypointPrefab != null) prefab = startWaypointPrefab;
            if (i == Path.Count - 1 && endWaypointPrefab != null) prefab = endWaypointPrefab;

            Instantiate(prefab, Path[i], Quaternion.identity);
        }
    }

    // Публичный метод для получения пути
    public List<Vector3> GetPath() => new List<Vector3>(Path); // копия, чтобы никто не сломал
}