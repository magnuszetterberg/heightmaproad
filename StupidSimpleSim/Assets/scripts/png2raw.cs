
using UnityEngine;

public class HeightmapImporter : MonoBehaviour
{
    public Texture2D heightMap; // Assign your heightmap in the inspector
    public float heightScale = 0.1f; // Adjust this value to scale the height of the terrain

    void Start()
    {
        if (heightMap == null)
        {
            Debug.LogError("Heightmap texture not assigned.");
            return;
        }

        if (!heightMap.isReadable)
        {
            Debug.LogError("Heightmap texture is not readable. Please enable 'Read/Write Enabled' in the texture import settings.");
            return;
        }

        // Get the terrain component
        Terrain terrain = GetComponent<Terrain>();
        if (terrain == null)
        {
            Debug.LogError("This script needs to be attached to a GameObject with a Terrain component.");
            return;
        }

        // Get the terrain data
        TerrainData terrainData = terrain.terrainData;
        int w2 = terrainData.heightmapResolution;
        int h2 = terrainData.heightmapResolution;

        // Create a new array for the heights
        float[,] heights = new float[w2, h2];

        // Loop through the image data
        for (int y = 0; y < h2; y++)
        {
            for (int x = 0; x < w2; x++)
            {
                // Sample the heightmap data
                float u = (float)x / (w2 - 1);
                float v = (float)y / (h2 - 1);
                float height = heightMap.GetPixelBilinear(u, v).grayscale * heightScale;
                heights[y, x] = height;
            }
        }

        // Assign the heights to the terrain
        terrainData.SetHeights(0, 0, heights);
    }
}
