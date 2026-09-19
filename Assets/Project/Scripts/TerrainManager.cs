using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    [SerializeField] private GameObject leftTerrainPrefab;
    [SerializeField] private GameObject mainTerrainPrefab;
    [SerializeField] private GameObject rightTerrainPrefab;
    [SerializeField] private Transform player;
    [SerializeField, Range(1, 3)] private int visibleTileRadius = 1;

    private readonly Dictionary<int, GameObject> spawnedTiles = new();
    private readonly List<int> tilesToRemove = new();

    private float terrainWidth;
    private int currentTileIndex = int.MinValue;

    private void Start()
    {
        terrainWidth = GetTotalWidth(mainTerrainPrefab);
        if (terrainWidth <= Mathf.Epsilon)
        {
            enabled = false;
            return;
        }

        RefreshVisibleTiles();
    }

    private void Update()
    {
        if (player == null)
        {
            return;
        }

        int playerTileIndex = Mathf.RoundToInt(player.position.x / terrainWidth);
        if (playerTileIndex != currentTileIndex)
        {
            RefreshVisibleTiles();
        }
    }

    private void RefreshVisibleTiles()
    {
        currentTileIndex = player == null ? 0 : Mathf.RoundToInt(player.position.x / terrainWidth);

        for (int index = currentTileIndex - visibleTileRadius; index <= currentTileIndex + visibleTileRadius; index++)
        {
            if (!spawnedTiles.ContainsKey(index))
            {
                SpawnTile(index);
            }
        }

        tilesToRemove.Clear();
        foreach (KeyValuePair<int, GameObject> tile in spawnedTiles)
        {
            if (Mathf.Abs(tile.Key - currentTileIndex) > visibleTileRadius)
            {
                tilesToRemove.Add(tile.Key);
            }
        }

        foreach (int index in tilesToRemove)
        {
            Destroy(spawnedTiles[index]);
            spawnedTiles.Remove(index);
        }
    }

    private void SpawnTile(int index)
    {
        GameObject prefab = index switch
        {
            < 0 => leftTerrainPrefab,
            > 0 => rightTerrainPrefab,
            _ => mainTerrainPrefab
        };

        if (prefab == null)
        {
            Debug.LogError($"Terrain prefab for tile {index} is not assigned.", this);
            return;
        }

        Vector3 position = new(index * terrainWidth, transform.position.y, transform.position.z);
        spawnedTiles[index] = Instantiate(prefab, position, Quaternion.identity, transform);
    }

    private static float GetTotalWidth(GameObject prefab)
    {
        if (prefab == null)
        {
            return 0f;
        }

        Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            Debug.LogError($"No Renderer found in terrain prefab '{prefab.name}'.", prefab);
            return 0f;
        }

        Bounds combinedBounds = renderers[0].bounds;
        for (int index = 1; index < renderers.Length; index++)
        {
            combinedBounds.Encapsulate(renderers[index].bounds);
        }

        return combinedBounds.size.x;
    }
}
