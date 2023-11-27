using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is responsible for generating the terrain using Perlin Noise
public class TerrainGenerator : MonoBehaviour
{
    // The size of the terrain
    [Header("Terrain Size")]
    public int width = 256;
    public int height = 256;
    // The scale of the terrain
    [Header("Terrain Scale")]
    public float scale = 20f;
    // The offset of the terrain
    [Header("Terrain Offset")]
    public float offsetX = 100f;
    public float offsetY = 100f;
    // The biome of the terrain
    [Header("Terrain Biome")]
    public Biome[] biomes;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh terrainMesh;

    private Vector3[] vertices;
    private int[] triangles;
    private Color[] colors;

    [System.Serializable]
    public class Biome
    {
        public string name;
        public float minHeight;
        public float maxHeight;
        public int heightModifier;
        public Color color;
    }

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


        GenerateTerrain();
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

        // Create a texture to store the colours
        Texture2D texture = new Texture2D(width, height);

        Debug.Log(terrainMesh.vertices.Length);

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
                float heightValue = Mathf.PerlinNoise((x + offsetX) / scale, (y + offsetY) / scale);
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
}