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
    List<Vector3> vertices;
    List<int> triangles;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        // Initialize lists
        vertices = new List<Vector3>();
        triangles = new List<int>();

        CreateMesh();
        UpdateMesh();
    }

    void CreateMesh()
    {
        for (int i = 0; i < terrainWidth; i++)
        {
            for (int j = 0; j < terrainHeight; j++)
            {
                // create vertices for each face
                vertices.Add(new Vector3(i * faceWidth, j * faceHeight, 0));
                vertices.Add(new Vector3(i * faceWidth, j * faceHeight + faceHeight, 0));
                vertices.Add(new Vector3(i * faceWidth + faceWidth, j * faceHeight + faceHeight, 0));
                vertices.Add(new Vector3(i * faceWidth + faceWidth, j * faceHeight, 0));

                // create triangles for each face; six vertices per face, two overlapping triangles
                triangles.Add((i * j) + 0);
                triangles.Add((i * j) + 1);
                triangles.Add((i * j) + 2);
                triangles.Add((i * j) + 0);
                triangles.Add((i * j) + 2);
                triangles.Add((i * j) + 3);
            }
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.Optimize();                        // Copilot recommended this and it can't possibly be this easy...        

        mesh.RecalculateNormals();
    }
    
}
