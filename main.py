
import numpy as np
import matplotlib.pyplot as plt
from noise import pnoise2
import heapq

# Parameters for the heightmap
width = 300
height = 300
scale = 300.0

# Parameters for Perlin noise
octaves = 6
persistence = 0.5
lacunarity = 2.0

# Generate heightmap using Perlin noise
def generate_heightmap(width, height, scale, octaves, persistence, lacunarity):
    heightmap = np.zeros((height, width))

    for i in range(height):
        for j in range(width):
            heightmap[i][j] = pnoise2(i / scale, 
                                       j / scale, 
                                       octaves=octaves, 
                                       persistence=persistence, 
                                       lacunarity=lacunarity, 
                                       repeatx=512, 
                                       repeaty=512, 
                                       base=42)
    # Normalize to 0-1
    min_val = np.min(heightmap)
    max_val = np.max(heightmap)
    heightmap = (heightmap - min_val) / (max_val - min_val)
    return heightmap
# A* pathfinding
def heuristic(a, b):
    return np.sqrt((b[0] - a[0]) ** 2 + (b[1] - a[1]) ** 2)

def a_star_search(heightmap, start, goal, height_penalty=10.0):
    neighbors = [(0, 1), (1, 0), (0, -1), (-1, 0), (1, 1), (-1, -1), (1, -1), (-1, 1)]  # 8-way connectivity

    open_set = []
    heapq.heappush(open_set, (0, start))
    came_from = {}
    g_score = {start: 0}
    f_score = {start: heuristic(start, goal)}

    while open_set:
        current = heapq.heappop(open_set)[1]

        if current == goal:
            path = []
            while current in came_from:
                path.append(current)
                current = came_from[current]
            path.append(current)
            return path[::-1]  # Return reversed path

        for i, j in neighbors:
            neighbor = (current[0] + i, current[1] + j)
            if 0 <= neighbor[0] < height and 0 <= neighbor[1] < width:
                # Calculate the slope (rise over run) and apply height penalty
                elevation_diff = abs(heightmap[current] - heightmap[neighbor])
                slope = elevation_diff / np.sqrt(i ** 2 + j ** 2)
                tentative_g_score = g_score[current] + np.sqrt((i ** 2 + j ** 2)) + (slope * height_penalty)

                if neighbor not in g_score or tentative_g_score < g_score[neighbor]:
                    came_from[neighbor] = current
                    g_score[neighbor] = tentative_g_score
                    f_score[neighbor] = tentative_g_score + heuristic(neighbor, goal)
                    heapq.heappush(open_set, (f_score[neighbor], neighbor))

    return []  # No path was found

# Draw the road on the heightmap
def add_road(heightmap, path, blend_factor=0.5):
    road_width = 1  # Width of the road in pixels
    road_elevation = 0.1  # Target elevation for the road

    for (y, x) in path:
        for dy in range(-road_width, road_width + 1):
            for dx in range(-road_width, road_width + 1):
                ny, nx = y + dy, x + dx
                if 0 <= ny < height and 0 <= nx < width:
                    # Blend road elevation with heightmap elevation
                    heightmap[ny, nx] = (blend_factor * road_elevation +
                                         (1 - blend_factor) * heightmap[ny, nx])
    return heightmap

# Main logic to generate the heightmap and road
heightmap = generate_heightmap(width, height, scale, octaves, persistence, lacunarity)
# Choose start and goal points more organically
start = (np.random.randint(0, height // 4), np.random.randint(0, width // 4))
goal = (np.random.randint(3 * height // 4, height), np.random.randint(3 * width // 4, width))

path = a_star_search(heightmap, start, goal, height_penalty=500.0)

if path:
    heightmap_with_road = add_road(heightmap, path, blend_factor=0.5)  # Use your chosen blend factor here
    plt.imshow(heightmap_with_road, cmap='gray')
    plt.axis('off')
    plt.savefig('heightmap_with_road.png', bbox_inches='tight', pad_inches=0)
    plt.close()
else:
    print("No path found")
