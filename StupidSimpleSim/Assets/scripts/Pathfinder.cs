// Pathfinder.cs
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder
{
    private Terrain terrain;
    private float[,] heightmap;
    private int width;
    private int height;
    private float heightPenalty;
    private Dictionary<Vector3Int, float> fScore;

    public Pathfinder(Terrain terrain, float heightPenalty)
    {
        this.terrain = terrain;
        TerrainData terrainData = terrain.terrainData;
        this.heightmap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        this.width = terrainData.heightmapResolution;
        this.height = terrainData.heightmapResolution;
        this.heightPenalty = heightPenalty;
        this.fScore = new Dictionary<Vector3Int, float>();
    }

    public List<Vector3> FindPath(Vector3 startWorldPos, Vector3 endWorldPos)
    {
        Vector3Int startNode = WorldToHeightmap(startWorldPos);
        Vector3Int endNode = WorldToHeightmap(endWorldPos);

        var openSet = new SortedSet<Vector3Int>(new ByFScoreComparer(fScore));
        var cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        var gScore = new Dictionary<Vector3Int, float>();

        gScore[startNode] = 0;
        fScore[startNode] = Heuristic(startNode, endNode);

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Vector3Int current = openSet.Min;
            if (current.Equals(endNode))
            {
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);
            foreach (var neighbor in GetNeighbors(current))
            {
                if (!gScore.ContainsKey(neighbor))
                    gScore[neighbor] = float.MaxValue;

                float tentativeGScore = gScore[current] + Heuristic(current, neighbor) +
                                        heightPenalty * Mathf.Abs(heightmap[current.x, current.y] - heightmap[neighbor.x, neighbor.y]);

                if (tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = tentativeGScore + Heuristic(neighbor, endNode);

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return new List<Vector3>();  // Path not found
    }

    private List<Vector3Int> GetNeighbors(Vector3Int node)
    {
        var neighbors = new List<Vector3Int>
        {
            new Vector3Int(node.x + 1, 0, node.z),
            new Vector3Int(node.x - 1, 0, node.z),
            new Vector3Int(node.x, 0, node.z + 1),
            new Vector3Int(node.x, 0, node.z - 1),
            // Add diagonal neighbors
            new Vector3Int(node.x + 1, 0, node.z + 1),
            new Vector3Int(node.x - 1, 0, node.z - 1),
            new Vector3Int(node.x + 1, 0, node.z - 1),
            new Vector3Int(node.x - 1, 0, node.z + 1),
        };

        // Remove any neighbors that are out of bounds
        neighbors.RemoveAll(n => n.x < 0 || n.x >= width || n.z < 0 || n.z >= height);
        return neighbors;
    }

    private float Heuristic(Vector3Int a, Vector3Int b)
    {
        return Vector3Int.Distance(a, b);
    }

    private float Distance(Vector3Int a, Vector3Int b)
    {
        // Adjust the distance calculation to account for diagonal movement and height differences
        float flatDistance = Vector2Int.Distance(new Vector2Int(a.x, a.z), new Vector2Int(b.x, b.z));
        float heightDifference = Mathf.Abs(heightmap[a.x, a.z] - heightmap[b.x, b.z]);
        // Calculate the cost considering the slope
        float slopeAngle = Mathf.Atan2(heightDifference, flatDistance) * Mathf.Rad2Deg;
        // Use an angle threshold to increase costs for steep slopes
        float slopeCost = slopeAngle > 20.0f ? heightPenalty : 10.0f;
        return flatDistance * slopeCost;
    }
    private List<Vector3> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int current)
    {
        var path = new List<Vector3>();
        while (cameFrom.ContainsKey(current))
        {
            path.Add(HeightmapToWorld(current));
            current = cameFrom[current];
        }
        path.Reverse();
        return path;
    }

    private Vector3Int WorldToHeightmap(Vector3 worldPos)
    {
        Debug.Log("WorldToHeightmap:");
        Vector3 relativePos = worldPos - terrain.transform.position;
        int x = Mathf.FloorToInt(relativePos.x / terrain.terrainData.size.x * width);
        int z = Mathf.FloorToInt(relativePos.z / terrain.terrainData.size.z * height);
        return new Vector3Int(x, 0, z);
    }

    private Vector3 HeightmapToWorld(Vector3Int heightmapPos)
    {
        float worldX = terrain.transform.position.x + heightmapPos.x / (float)width * terrain.terrainData.size.x;
        float worldZ = terrain.transform.position.z + heightmapPos.z / (float)height * terrain.terrainData.size.z;
        float worldY = terrain.SampleHeight(new Vector3(worldX, 0, worldZ));
        return new Vector3(worldX, worldY, worldZ);
    }

    private class ByFScoreComparer : IComparer<Vector3Int>
    {
        private readonly Dictionary<Vector3Int, float> fScore;

        public ByFScoreComparer(Dictionary<Vector3Int, float> fScore)
        {
            this.fScore = fScore;
        }

        public int Compare(Vector3Int x, Vector3Int y)
        {
            float xScore = fScore.TryGetValue(x, out float xF) ? xF : float.MaxValue;
            float yScore = fScore.TryGetValue(y, out float yF) ? yF : float.MaxValue;
            return xScore.CompareTo(yScore);
        }
    }
}