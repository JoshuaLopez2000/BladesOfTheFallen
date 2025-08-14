using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public GameObject leftTerrainPrefab, mainTerrainPrefab, rightTerrainPrefab;
    public Transform player;
    private float terrainWidth;
    private float lastRightX;
    private float lastLeftX;

    void Start()
    {
        terrainWidth = GetTotalWidth(mainTerrainPrefab);

        Instantiate(leftTerrainPrefab, Vector3.left * terrainWidth, Quaternion.identity);
        Instantiate(mainTerrainPrefab, Vector3.zero, Quaternion.identity);
        Instantiate(rightTerrainPrefab, Vector3.right * terrainWidth, Quaternion.identity);

        lastRightX = terrainWidth;
        lastLeftX = -terrainWidth;
    }

    void Update()
    {
        if (player.position.x > lastRightX - (terrainWidth / 2))
        {
            lastRightX += terrainWidth;
            Instantiate(rightTerrainPrefab, new Vector3(lastRightX, 0, 0), Quaternion.identity);
        }

        if (player.position.x < lastLeftX + (terrainWidth / 2))
        {
            lastLeftX -= terrainWidth;
            Instantiate(leftTerrainPrefab, new Vector3(lastLeftX, 0, 0), Quaternion.identity);
        }
    }

    float GetTotalWidth(GameObject prefab)
    {
        Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            Debug.LogError("No Renderer found in prefab: " + prefab.name);
            return 0f;
        }

        Bounds combinedBounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            combinedBounds.Encapsulate(renderers[i].bounds);
        }
        return combinedBounds.size.x;
    }
}
