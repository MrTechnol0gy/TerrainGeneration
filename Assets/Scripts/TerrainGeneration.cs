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
    private float heightValue;

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
        AdjustHeight();
        UpdateMesh();
    }

    void CreateMesh()
    {
        for (int i = 0; i < terrainWidth; i++)
        {
            for (int j = 0; j < terrainHeight; j++)
            {
                heightValue = RealPerlinNoise(Mathf.PerlinNoise(i + 0.1f, j + 0.1f));
                Debug.Log(heightValue);
                // create vertices for each face
                // 0,0 is bottom left corner
                // 0,1 is top left corner
                // 1,1 is top right corner
                // 1,0 is bottom right corner

                vertices.Add(new Vector3(i * faceWidth, 0, j * faceHeight));
                vertices.Add(new Vector3(i * faceWidth, 0, j * faceHeight + faceHeight));
                vertices.Add(new Vector3(i * faceWidth + faceWidth, 0, j * faceHeight + faceHeight));
                vertices.Add(new Vector3(i * faceWidth + faceWidth, 0, j * faceHeight));

                // create triangles for each face; six vertices per face, two overlapping triangles
                // 0,1,2 and 0,2,3
                // The first triangle is the bottom left, top left, top right
                // The second triangle is the bottom left, top right, bottom right
                // In this case, the triangles are counter-clockwise, so the normal is pointing up
                // The 4 * i * terrainHeight is to account for the previous faces
                // The 4 * j is to account for the previous vertices in the current face
                // The 0,1,2 and 0,2,3 are the indices of the vertices in the vertices list

                // The first triangle:
                triangles.Add(0 + 4 * i * terrainHeight + 4 * j);
                triangles.Add(1 + 4 * i * terrainHeight + 4 * j);
                triangles.Add(2 + 4 * i * terrainHeight + 4 * j);
                // The second triangle:
                triangles.Add(0 + 4 * i * terrainHeight + 4 * j);
                triangles.Add(2 + 4 * i * terrainHeight + 4 * j);
                triangles.Add(3 + 4 * i * terrainHeight + 4 * j);
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
    
    float RealPerlinNoise(float value)
    {
        value = value * 2 - 1;
        return value;
    }

    void AdjustHeight()
    {
        // Access and modify the height values of the vertices
        for (int i = 0; i < vertices.Count; i++)
        {
            // Access a specific vertex
            Vector3 vertex = vertices[i];

            // Modify the height value
            vertex.y = RealPerlinNoise(Mathf.PerlinNoise(vertex.x, vertex.z));

            // Assign the modified vertex back to the list
            vertices[i] = vertex;
        }
    }
}
