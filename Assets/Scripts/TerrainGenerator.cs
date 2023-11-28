using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This class is responsible for generating the terrain using Perlin Noise
public class TerrainGenerator : MonoBehaviour
{
    // The size of the terrain
    [Header("Terrain Size")]
    public int width = 128;
    public int height = 128;
    private int numOfTriangles = 6;
    // The scale of the terrain
    [Header("Terrain Modifiers")]
    public float scale = 20f;           // The scale of the terrain; Scale affects the frequency of the Perlin Noise
    public int octaves = 4;             // The number of octaves of Perlin Noise to use; More octaves = more detail
    public float persistence = 0.5f;    // The persistence of the Perlin Noise; Persistence affects the amplitude of the Perlin Noise
    public float lacunarity = 2f;       // The lacunarity of the Perlin Noise; Lacunarity affects the frequency of the Perlin Noise
    // The biome of the terrain
    [Header("Terrain Biome")]
    public Biome[] biomes;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh terrainMesh;

    private Vector3[] vertices;
    private int[] triangles;

    [System.Serializable]
    public class Biome
    {
        public string name;
        public float minHeight;
        public float maxHeight;
        public Material material;
    }

    private float heightValue;
    Texture2D texture;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();  // Add MeshRenderer initialization

        if (meshFilter == null)
        {
            Debug.Log("Mesh filter not found");
            meshFilter = gameObject.AddComponent<MeshFilter>();  // Add MeshFilter component if not already present
        }

        if (meshRenderer == null)
        {
            Debug.Log("Mesh renderer not found");
            meshRenderer = gameObject.AddComponent<MeshRenderer>();  // Add MeshRenderer component if not already present
        }
       
        // Generate the terrain mesh
        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        // Create the terrain mesh
        terrainMesh = CreateTerrainMesh();
        // Set the mesh filter's mesh to the terrain mesh
        meshFilter.mesh = terrainMesh;

        // if the mesh filter doesn't have a mesh...
        if (meshFilter.mesh == null)
        {
            Debug.Log("Mesh filter doesn't have a mesh");
        }
    }

    Mesh CreateTerrainMesh()
    {
        terrainMesh = new Mesh();
        vertices = new Vector3[(width + 1) * (height + 1)];
        triangles = new int[width * height * numOfTriangles];
        int vertexIndex = 0;
        int triangleIndex = 0;

        for (int y = 0; y <= height; y++)
        {
            for (int x = 0; x <= width; x++)
            {
                // Utility function to generate the height of the terrain
                heightValue = GenerateHeight(x, y);
                
                // Debug.Log("Height value: " + heightValue);

                vertices[vertexIndex] = new Vector3(x, heightValue, y);

                if (x < width && y < height)
                {
                    triangles[triangleIndex] = vertexIndex;
                    triangles[triangleIndex + 1] = vertexIndex + width + 1;
                    triangles[triangleIndex + 2] = vertexIndex + 1;
                    triangles[triangleIndex + 3] = vertexIndex + 1;
                    triangles[triangleIndex + 4] = vertexIndex + width + 1;
                    triangles[triangleIndex + 5] = vertexIndex + width + 2;
                    triangleIndex += 6;
                }

                vertexIndex++;
            }
        }

        terrainMesh.Clear();
        terrainMesh.vertices = vertices;
        terrainMesh.triangles = triangles;
        terrainMesh.RecalculateNormals();

        return terrainMesh;
    }

    float GenerateHeight(float x, float y)
    {
        float heightValue = 0;

        float frequency = 1;
        float amplitude = 1;

        for (int i = 0; i < octaves; i++)
        {
            float xCoord = x / scale * frequency;
            float yCoord = y / scale * frequency;

            float perlinValue = UnityPerlinToRealPerlin(Mathf.PerlinNoise(xCoord, yCoord));

            heightValue += perlinValue * amplitude;

            frequency *= lacunarity;
            amplitude *= persistence;
        }

        return heightValue;
    }

    Biome GetBiome(float heightValue)
    {
        foreach (Biome biome in biomes)
        {
            if (heightValue >= biome.minHeight && heightValue <= biome.maxHeight)
            {
                Debug.Log($"Height value: {heightValue}, Biome Range: {biome.minHeight} to {biome.maxHeight}");
                return biome;
            }
        }

        // Debug.Log("No biome found for height value: " + heightValue);
        return null;
    }

    Texture2D CreateBiomeTexture()
    {
        texture = new Texture2D(width, height);

        Color[] colors = new Color[width * height];
        int colorIndex = 0;

        for (int y = 0; y <= height; y++)
        {
            for (int x = 0; x <= width; x++)
            {
                heightValue = GenerateHeight(x, y);
                Biome biome = GetBiome(heightValue);
                colors[colorIndex] = biome.material.color;
                colorIndex++;
            }
        }

        texture.SetPixels(colors);
        texture.Apply();

        return texture;
    }

    public static float UnityPerlinToRealPerlin(float unityPerlin)
    {
        // Map Unity's Perlin range (0 to 1) to the range of real Perlin Noise (-1 to 1)
        return unityPerlin * 2 - 1;
    }
}