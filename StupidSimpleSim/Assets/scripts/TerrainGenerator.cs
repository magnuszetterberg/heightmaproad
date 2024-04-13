
using UnityEngine;
using System.Collections.Generic;

public class TerrainGenerator : MonoBehaviour
{
    public int depth = 20;
    public int width = 256;
    public int height = 256;
    public float scale = 20f;
    public Terrain terrain;
    public RoadGenerator roadGenerator; // Reference to the RoadGenerator script
    public int roadWidth = 5; // Width of the road
    public float roadElevation = 0.1f; // Elevation of the road

    private void Awake()
    {
        // Ensure the terrain component is assigned
        // terrain = GetComponent<Terrain>();
        // if (terrain == null)
        // {
        //     Debug.LogError("Terrain component not found on the GameObject.");
        // }
    }

    public void SetSeedAndGenerate(int seed)
    {
        if (terrain != null)
        {
            // Generate the terrain
            terrain.terrainData = GenerateTerrain(terrain.terrainData, seed);

            // After generating the terrain, generate the road if RoadGenerator is assigned
            if (roadGenerator != null)
            {
                // Get the path points for the road here (needs implementation)
                List<Vector3> pathPoints = GetPathPoints(); 
                roadGenerator.GenerateRoad(terrain, pathPoints, roadWidth, roadElevation);
            }
            else
            {
                Debug.LogError("RoadGenerator script reference not set.");
            }
        }
    }

    TerrainData GenerateTerrain(TerrainData terrainData, int seed)
    {
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, depth, height);
        terrainData.SetHeights(0, 0, GenerateHeights(seed));
        return terrainData;
    }

    float[,] GenerateHeights(int seed)
    {
        float[,] heights = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = CalculateHeight(x, y, seed);
            }
        }
        return heights;
    }

float CalculateHeight(int x, int y, int seed)
{
    float xNormalized = (float)x / (width - 1);
    float yNormalized = (float)y / (height - 1);
    float xCoord = xNormalized * scale + seed;
    float yCoord = yNormalized * scale + seed;

    // Island mask - simple radial gradient from the center of the map
    float distanceToCenter = Mathf.Sqrt((xNormalized - 0.5f) * (xNormalized - 0.5f) + (yNormalized - 0.5f) * (yNormalized - 0.5f));
    float islandMask = 1.0f - Mathf.Clamp01(distanceToCenter / 0.5f);

    float totalHeight = 0;
    float frequency = 1;
    float amplitude = 1;
    float maxAmplitude = 0; // Will accumulate the maximum amplitude for normalization
    float persistence = 0.5f; // Controls the amplitude falloff per octave
    int octaves = 6; // The number of layers of noise

    for (int i = 0; i < octaves; i++)
    {
        float perlinValue = Mathf.PerlinNoise(xCoord * frequency, yCoord * frequency);
        totalHeight += perlinValue * amplitude;
        maxAmplitude += amplitude;
        
        amplitude *= persistence;
        frequency *= 2;
    }

    totalHeight /= maxAmplitude; // Normalize to [0, 1]
    totalHeight *= islandMask; // Apply the island mask

    return totalHeight; // Return the calculated height
}


private List<Vector3> GetPathPoints()
{
    // Calculate the center of the terrain in world space
    Vector3 terrainCenter = new Vector3(terrain.transform.position.x + terrain.terrainData.size.x / 2, 
                                        0, 
                                        terrain.transform.position.z + terrain.terrainData.size.z / 2);

    // Calculate the start and end points for the road, keeping it in the middle
    Vector3 startPoint = new Vector3(terrainCenter.x, 0, terrain.transform.position.z);
    Vector3 endPoint = new Vector3(terrainCenter.x, 0, terrain.transform.position.z + terrain.terrainData.size.z);

    // Adjust y (height) of startPoint and endPoint based on the terrain height at those points
    startPoint.y = terrain.SampleHeight(startPoint) + roadElevation;
    endPoint.y = terrain.SampleHeight(endPoint) + roadElevation;

    // Return a list with just two points for a simple straight road
    return new List<Vector3> { startPoint, endPoint };
}
}
