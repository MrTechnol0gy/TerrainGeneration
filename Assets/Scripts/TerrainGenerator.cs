using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is responsible for generating the terrain using Perlin Noise
public class TerrainGenerator : MonoBehaviour
{
    // The size of the terrain
    [Header("Terrain Size")]
    public static int width = 128;
    public static int height = 128;
    // The scale of the terrain
    [Header("Terrain Scale")]
    public static float scale = 20f;
    // The biome of the terrain
    [Header("Terrain Biome")]
    public Biome[] biomes;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh terrainMesh;

    // Create height map based on Perlin Noise
    private float[,] heightMap;

    private Vector3[] vertices;
    private int[] triangles;

    [System.Serializable]
    public class Biome
    {
        public string name;
        public float minHeight;
        public float maxHeight;
        public int heightModifier;
        public Color color;
    }

    // Create biome map based on height map
    private Biome[,] biomeMap = new Biome[width, height];

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();  // Add MeshRenderer initialization

        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();  // Add MeshFilter component if not already present
        }

        if (meshRenderer == null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();  // Add MeshRenderer component if not already present
        }

        
        // Create an array to store biome information
        biomeMap = new Biome[width, height];
        // Generate the height map using Perlin Noise
        heightMap = GeneratePerlinNoiseMap(width, height, scale);
        // Assign biomes to the terrain based on the height map
        AssignBiomes(heightMap);
        // Create a texture based on the biome map
        meshRenderer.material.mainTexture = CreateBiomeTexture();
        // Generate the terrain mesh
        GenerateTerrain();
    }

    // Generate the height map using Perlin Noise
    public static float[,] GeneratePerlinNoiseMap(int width, int height, float scale)
    {
        float[,] noiseMap = new float[width, height];

        // Loop through each pixel of the map
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Calculate sample indices based on the scale
                float sampleX = x / scale;
                float sampleY = y / scale;

                // Generate the noise value using Perlin Noise
                float realPerlinValue = UnityPerlinToRealPerlin(Mathf.PerlinNoise(sampleX, sampleY));

                // Set the noise map value to the perlin value
                noiseMap[x, y] = realPerlinValue;
            }
        }

        return noiseMap;
    }

    // Assign biomes to the terrain based on the height map
    public void AssignBiomes(float[,] heightMap)
    {
        // Loop through each pixel of the map
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Loop through each biome
                for (int i = 0; i < biomes.Length; i++)
                {
                    // If the height map value is between the min and max height of the biome
                    if (heightMap[x, y] >= biomes[i].minHeight && heightMap[x, y] <= biomes[i].maxHeight)
                    {
                        // Assign the biome to the pixel
                        biomeMap[x, y] = biomes[i];
                    }
                }
            }
        }
    }

    // Create a texture based on the biome map
    public Texture2D CreateBiomeTexture()
    {
        Texture2D texture = new Texture2D(width + 1, height + 1);

        // Loop through each pixel of the map
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Debug.Log(biomeMap[x, y]);
                Biome currentBiome = biomeMap[x, y];
                // Set the pixel color to the biome color
                Debug.Log(currentBiome.color);
                texture.SetPixel(x, y, currentBiome.color);
            }
        }

        texture.Apply();

        return texture;
    }

    void GenerateTerrain()
    {
        terrainMesh = CreateTerrainMesh();
        meshFilter.mesh = terrainMesh;

        // if the mesh filter doesn't have a mesh...
        if (meshFilter.mesh == null)
        {
            Debug.Log("Mesh filter doesn't have a mesh");
        }
    }

    Mesh CreateTerrainMesh()
    {
        Mesh mesh = new Mesh();
        vertices = new Vector3[(width + 1) * (height + 1)];
        triangles = new int[width * height * 6];
        int vertexIndex = 0;
        int triangleIndex = 0;

        for (int y = 0; y <= height; y++)
        {
            for (int x = 0; x <= width; x++)
            {
                float heightValue = UnityPerlinToRealPerlin(Mathf.PerlinNoise(x  / scale, y / scale));
                Debug.Log("Height value: " + heightValue);

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

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    public static float UnityPerlinToRealPerlin(float unityPerlin)
    {
        // Map Unity's Perlin range (0 to 1) to the range of real Perlin Noise (-1 to 1)
        return unityPerlin * 2 - 1;
    }

}