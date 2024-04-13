
// RoadGenerator.cs
using UnityEngine;
using System.Collections.Generic;

public class RoadGenerator : MonoBehaviour
{
    // Method called externally to start road generation
    public void GenerateRoad(Terrain terrain, List<Vector3> pathPoints, int roadWidth, float roadElevationOffset)
    {
        AddRoad(terrain, pathPoints, roadWidth, roadElevationOffset);
        Debug.Log("Road generated!");
    }

    // Method that generates the road on the terrain
    private void AddRoad(Terrain terrain, List<Vector3> pathPoints, int roadWidth, float roadElevationOffset)
    {
        Debug.Log("Adding road to terrain...");
        TerrainData terrainData = terrain.terrainData;
        int heightmapWidth = terrainData.heightmapResolution;
        int heightmapHeight = terrainData.heightmapResolution;

        // Get the current heights of the terrain heightmap
        float[,] heights = terrainData.GetHeights(0, 0, heightmapWidth, heightmapHeight);

        // Iterate over each segment in the path
        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            Vector3 pointA = pathPoints[i];
            Vector3 pointB = pathPoints[i + 1];

            // Determine the number of steps based on the distance between points A and B
            int steps = Mathf.CeilToInt(Vector3.Distance(pointA, pointB) * heightmapWidth / terrainData.size.x);
            for (int s = 0; s <= steps; s++)
            {
                // Lerp between point A and B to find the intermediate point
                float t = (float)s / steps;
                Vector3 interpolatedPoint = Vector3.Lerp(pointA, pointB, t);

                // Convert the interpolated point from world space to heightmap space
                Vector3 terrainLocalPos = interpolatedPoint - terrain.transform.position;
                float normX = terrainLocalPos.x / terrainData.size.x;
                float normZ = terrainLocalPos.z / terrainData.size.z;
                int heightmapX = Mathf.RoundToInt(normX * heightmapWidth);
                int heightmapZ = Mathf.RoundToInt(normZ * heightmapHeight);

                // Make sure we're not going out of bounds
                heightmapX = Mathf.Clamp(heightmapX, 0, heightmapWidth - 1);
                heightmapZ = Mathf.Clamp(heightmapZ, 0, heightmapHeight - 1);

                // Determine the range of heightmap coordinates to affect based on the road width
                int minX = Mathf.Max(0, heightmapX - roadWidth / 2);
                int maxX = Mathf.Min(heightmapWidth - 1, heightmapX + roadWidth / 2);
                int minZ = Mathf.Max(0, heightmapZ - roadWidth / 2);
                int maxZ = Mathf.Min(heightmapHeight - 1, heightmapZ + roadWidth / 2);

                // Find the average terrain height at this segment and apply the offset to get the desired road height
                float sumHeight = 0f;
                int count = 0;
                for (int z = minZ; z <= maxZ; z++)
                {
                    for (int x = minX; x <= maxX; x++)
                    {
                        sumHeight += heights[z, x];
                        count++;
                    }
                }
                float averageHeight = (sumHeight / count) + (roadElevationOffset / terrainData.size.y);

                // Set the height for the road, ensuring it's smooth and follows the terrain
                for (int z = minZ; z <= maxZ; z++)
                {
                    for (int x = minX; x <= maxX; x++)
                    {
                        // Apply a smoothing factor to avoid sharp changes in the road height
                        heights[z, x] = Mathf.Lerp(heights[z, x], averageHeight, 0.8f);
                    }
                }
            }
        }

        // Apply the modified heights back to the terrain heightmap
        terrainData.SetHeights(0, 0, heights);
    }
}
