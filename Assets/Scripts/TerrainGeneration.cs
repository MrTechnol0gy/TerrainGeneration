using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]                  // Require MeshFilter component
public class TerrainGeneration : MonoBehaviour
{
    [Header("Terrain Size")]
    public int terrainWidth;
    public int terrainHeight;

    [Header("Height Variables")]
    public float scale;
    public float multiplier;

    Mesh mesh;
    List<Vector3> vertices;
    List<int> triangles;
    List<float> heightMap;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        // Initialize lists
        vertices = new List<Vector3>();
        triangles = new List<int>();

        // Create mesh, which is flat
        CreateMesh();
        // Generate heightmap
        heightMap = GenerateHeightMap(terrainWidth, terrainHeight, scale, multiplier);
        // Apply heightmap to vertices
        ApplyHeightmap(heightMap, terrainWidth, terrainHeight, vertices);
        // Update mesh with all changes
        UpdateMesh();
    }

    void Update()
    {
        // If player presses escape, quit the game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void CreateMesh()
    {
        for (int x = 0; x < terrainWidth; x++)
        {
            for (int z = 0; z < terrainHeight; z++)
            {
                // Create a vertex at each point in the grid
                // The x and z coordinates are the same as the loop
                // The y coordinate is 0; will be modified later
                float y = 0f;
                Vector3 vertex = new Vector3(x, y, z);
                vertices.Add(vertex);
            }
        }

        // create triangles for each face
        for (int x = 0; x < terrainWidth - 1; x++)
        {
            for (int z = 0; z < terrainHeight - 1; z++)
            {
                // Six vertices per face, two overlapping triangles
                // 0,1,2 and 0,2,3
                // The first triangle is the bottom left, top left, top right
                // The second triangle is the bottom left, top right, bottom right
                // The 0,1,2 and 0,2,3 are the indices of the vertices in the vertices list

                int topLeft = x + z * terrainWidth;
                int topRight = (x + 1) + z * terrainWidth;
                int bottomLeft = x + (z + 1) * terrainWidth;
                int bottomRight = (x + 1) + (z + 1) * terrainWidth;

                // First triangle
                triangles.Add(topLeft);
                triangles.Add(topRight);
                triangles.Add(bottomLeft);

                // Second triangle
                triangles.Add(topRight);
                triangles.Add(bottomRight);
                triangles.Add(bottomLeft);
            }
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        
        mesh.RecalculateNormals();
    }

    void ApplyHeightmap(List<float> heightmap, int width, int length, List<Vector3> vertices) 
    {
        for (int x = 0; x < width; x++) 
        {
            for (int z = 0; z < length; z++) 
            {
                // Get the index of the vertex in the vertices list
                int index = x + z * width;
                Vector3 vertex = vertices[index];
                // Change the y coordinate of the vertex to the heightmap value
                vertex.y = heightmap[index];
                // Update the vertex in the vertices list
                vertices[index] = vertex;
            }
        }
    }
    
    float RealPerlinNoise(float value)
    {
        value = value * 2 - 1;
        return value;
    }

    List<float> GenerateHeightMap(int width, int height, float scale, float multiplier)
    {
        heightMap = new List<float>();
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                // Perlin noise is a value between 0 and 1 in Unity, which is wrong
                float xCoord = (float)x / width * scale;
                float zCoord = (float)z / height * scale;

                // 
                float y = Mathf.PerlinNoise(xCoord, zCoord);
                // y = y * 2 - 1 wrapper, to fix Unity's Perlin noise
                y = RealPerlinNoise(y);
                y *= multiplier;
                // Add the y value to the heightmap list
                heightMap.Add(y);
            }
        }
        return heightMap;
    }
}
