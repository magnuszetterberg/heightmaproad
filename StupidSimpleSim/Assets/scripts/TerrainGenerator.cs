// TerrainGenerator.cs
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
        // terrain = GetComponent<Terrain>();
        // if (terrain == null)
        // {
        //     Debug.LogError("Terrain component not found on the GameObject.");
        // }
    }

    public void SetSeedAndGenerate(int seed)
    {
        terrain.terrainData = GenerateTerrain(terrain.terrainData, seed);

        if (roadGenerator != null)
        {
            Pathfinder pathfinder = new Pathfinder(terrain, roadElevation);
            List<Vector3> pathPoints = GetPathPoints(pathfinder);
            roadGenerator.GenerateRoad(terrain, pathPoints, roadWidth, roadElevation);
        }
        else
        {
            Debug.LogError("RoadGenerator script reference not set.");
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



private List<Vector3> GetPathPoints(Pathfinder pathfinder)
{
    // Calculate the middle point of the terrain in world space
    Vector3 terrainCenter = new Vector3(terrain.transform.position.x + terrain.terrainData.size.x / 2, 
                                        0, 
                                        terrain.transform.position.z + terrain.terrainData.size.z / 2);

    // Randomize the corners of the loop around the center
    List<Vector3> corners = new List<Vector3>
    {
        new Vector3(terrainCenter.x - width / 4 + Random.Range(-width / 8, width / 8), 0, terrainCenter.z - height / 4 + Random.Range(-height / 8, height / 8)),
        new Vector3(terrainCenter.x + width / 4 + Random.Range(-width / 8, width / 8), 0, terrainCenter.z - height / 4 + Random.Range(-height / 8, height / 8)),
        new Vector3(terrainCenter.x + width / 4 + Random.Range(-width / 8, width / 8), 0, terrainCenter.z + height / 4 + Random.Range(-height / 8, height / 8)),
        new Vector3(terrainCenter.x - width / 4 + Random.Range(-width / 8, width / 8), 0, terrainCenter.z + height / 4 + Random.Range(-height / 8, height / 8))
    };

    // Close the loop by connecting the last point back to the first
    corners.Add(corners[0]);

    // This list will hold the final set of path points for the road
    List<Vector3> pathPoints = new List<Vector3>();

    // Use the Pathfinder to find a path between each pair of points
    for (int i = 0; i < corners.Count - 1; i++)
    {
        Vector3 startPoint = corners[i];
        Vector3 endPoint = corners[i + 1];

        // Use the pathfinder to find the path between corners
        List<Vector3> segmentPathPoints = pathfinder.FindPath(startPoint, endPoint);

        // If not the first segment, remove the first point to avoid duplication
        if (i > 0)
        {
            segmentPathPoints.RemoveAt(0);
        }

        pathPoints.AddRange(segmentPathPoints);
    }

    // Adjust the height of each path point based on the terrain's height
    for (int i = 0; i < pathPoints.Count; i++)
    {
        Vector3 point = pathPoints[i];
        point.y = terrain.SampleHeight(point) + roadElevation;
        pathPoints[i] = point;
    }

    return pathPoints;
}


}
