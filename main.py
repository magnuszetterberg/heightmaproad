import numpy as np
import matplotlib.pyplot as plt
from noise import pnoise2
from heapq import heappush, heappop

# Configuration parameters
width = 512
height = 512
scale = 100.0
octaves = 6
persistence = 0.5
lacunarity = 2.0
custom_seed = 4254
height_penalty = 10.0
path_deviation_weight = 1000000.0
min_loop_length = 700  # The minimum number of points in the loop
max_attempts = 20  # Maximum attempts to find a loop with the minimum length


# Function to generate the heightmap using Perlin noise
def generate_heightmap(w, h, scale, octaves, persistence, lacunarity, seed):
    heightmap = np.zeros((h, w))
    for i in range(h):
        for j in range(w):
            heightmap[i][j] = pnoise2(i / scale, j / scale, octaves=octaves,
                                       persistence=persistence, lacunarity=lacunarity,
                                       repeatx=1024, repeaty=1024, base=seed)
    # Normalize the heightmap to range [0, 1]
    min_val, max_val = heightmap.min(), heightmap.max()
    heightmap = (heightmap - min_val) / (max_val - min_val)
    return heightmap

# A* pathfinding function
def a_star_search(heightmap, start, goal, avoid=None, height_penalty=10.0, path_deviation_weight=2.0):
    def heuristic(a, b):
        return np.sqrt((b[0] - a[0])**2 + (b[1] - a[1])**2)
    
    def get_neighbors(node):
        x, y = node
        steps = [(0, 1), (1, 0), (0, -1), (-1, 0), (1, 1), (-1, -1), (1, -1), (-1, 1)]
        return [(x + dx, y + dy) for dx, dy in steps if 0 <= x + dx < height and 0 <= y + dy < width]
    
    def avoid_penalty(node, avoid_path, weight):
        if avoid_path and node in avoid_path:
            return weight
        return 0
    
    close_set = set()
    came_from = {}
    gscore = {start: 0}
    fscore = {start: heuristic(start, goal)}
    oheap = []
    
    heappush(oheap, (fscore[start], start))
    
    while oheap:
        current = heappop(oheap)[1]
        
        if current == goal:
            data = []
            while current in came_from:
                data.append(current)
                current = came_from[current]
            return data[::-1]
        
        close_set.add(current)
        for neighbor in get_neighbors(current):
            if neighbor in close_set:
                continue

            tentative_g_score = gscore[current] + heuristic(current, neighbor)
            tentative_g_score += height_penalty * abs(heightmap[current] - heightmap[neighbor])
            tentative_g_score += avoid_penalty(neighbor, avoid, path_deviation_weight)
            
            if neighbor not in gscore or tentative_g_score < gscore[neighbor]:
                came_from[neighbor] = current
                gscore[neighbor] = tentative_g_score
                fscore[neighbor] = tentative_g_score + heuristic(neighbor, goal)
                heappush(oheap, (fscore[neighbor], neighbor))
                    
    return []


def add_road(heightmap, path, road_width=5, road_offset=1.):
    # Calculate the elevation for the road based on the terrain height at each point plus a fixed offset
    road_elevations = [heightmap[point] + road_offset for point in path]

    # Apply the road elevation to the points around each path point based on the road width
    for i, point in enumerate(path):
        road_elevation = road_elevations[i]
        for dy in range(-road_width, road_width + 1):
            for dx in range(-road_width, road_width + 1):
                ny, nx = point[0] + dy, point[1] + dx
                if 0 <= ny < height and 0 <= nx < width:
                    # Set the road elevation, ensuring it's not lower than the existing terrain
                    heightmap[ny, nx] = max(heightmap[ny, nx], road_elevation)
    return heightmap

def get_random_point(padding=20):
    x = np.random.randint(padding, width - padding)
    y = np.random.randint(padding, height - padding)
    return (y, x)



if __name__ == "__main__":
    # Generate the heightmap and add the road
    heightmap = generate_heightmap(width, height, scale, octaves, persistence, lacunarity, custom_seed)

    path_found = False
    attempts = 0

    while not path_found and attempts < max_attempts:
        # Randomly select start and intermediate points for the loop
        start = get_random_point()
        intermediate = get_random_point()

        # Find the path from start to intermediate
        path1 = a_star_search(heightmap, start, intermediate, height_penalty=height_penalty)

        # Find the path from intermediate back to start
        if path1:
            path2 = a_star_search(heightmap, intermediate, start, avoid=path1, height_penalty=height_penalty, path_deviation_weight=path_deviation_weight)
            
            if path2:
                # Combine the two paths to create a loop
                looped_path = path1[:-1] + path2  # Avoid duplicating the intermediate point

                # Check the loop length
                if len(looped_path) >= min_loop_length:
                    path_found = True
                    # Add the road to the heightmap
                    heightmap_with_road = add_road(heightmap, looped_path)
                    # Re-normalize the heightmap after adding the road
                    min_val, max_val = heightmap_with_road.min(), heightmap_with_road.max()
                    heightmap_normalized = (heightmap_with_road - min_val) / (max_val - min_val)
                # Save the normalized heightmap with the road to a PNG file
                    plt.imshow(heightmap_normalized, cmap="gray")
                    plt.axis("off")  # Hide the axis
                    plt.savefig('heightmap_with_road.png', bbox_inches='tight', pad_inches=0)
                    plt.close()
                    print("The heightmap with the road has been saved as 'heightmap_with_road.png'.")
                else:
                    print(f"Loop found but too short ({len(looped_path)} points), trying again...")
            else:
                print("No return path found.")
        else:
            print("No path found.")
        attempts += 1

    if not path_found:
        print("Failed to find a suitable loop within the maximum number of attempts.")
