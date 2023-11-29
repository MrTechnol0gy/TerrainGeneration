using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]                  // Require MeshFilter component
public class TerrainGeneration : MonoBehaviour
{
    [Header("Face Size")]
    public int faceWidth;
    public int faceHeight;

    [Header("Terrain Size")]
    public int terrainWidth;
    public int terrainHeight;

    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateMesh();
    }

    void CreateShape()
    {
        vertices = new Vector3[]
        {
            new Vector3(0, 0, 0),           // Bottom left
            new Vector3(0, 0, faceWidth),       // Top left
            new Vector3(faceHeight, 0, 0),      // Bottom right
            new Vector3(faceHeight, 0, faceWidth)   // Top right
        };
        
        triangles = new int[]
        {
            0, 1, 2,    // First triangle
            2, 1, 3     // Second triangle
        };
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }
    
}
