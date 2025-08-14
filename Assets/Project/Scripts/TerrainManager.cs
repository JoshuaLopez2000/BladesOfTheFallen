using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public GameObject leftTerrainPrefab, mainTerrainPrefab, rightTerrainPrefab;

    void Start()
    {
        float terrainWidth = GetTotalWidth(mainTerrainPrefab);

        Instantiate(leftTerrainPrefab, Vector3.left * terrainWidth, Quaternion.identity);
        Instantiate(mainTerrainPrefab, Vector3.zero, Quaternion.identity);
        Instantiate(rightTerrainPrefab, Vector3.right * terrainWidth, Quaternion.identity);
    }

    float GetTotalWidth(GameObject prefab)
    {
        Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            Debug.LogError("No Renderer found in prefab: " + prefab.name);
            return 0f;
        }

        // Combinar todos los bounds
        Bounds combinedBounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            combinedBounds.Encapsulate(renderers[i].bounds);
        }

        return combinedBounds.size.x;
    }
}
