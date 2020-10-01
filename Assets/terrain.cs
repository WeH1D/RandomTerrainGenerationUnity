using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class terrain : MonoBehaviour
{

    Mesh mesh;//==> Mesh itself
    Vector3[] vertices; //==> Points of triangles
    int[] triangles; //==> Triangles that make a mesh


    public float xOffset;
    public float zOffset;

    public int xSize = 100; //==> Width of the mesh
    public int zSize = 100; // ==> Length of the mesh
    [Range(-100, 100)]
    public float amplitude = 16.4f;
    [Range(-.2f, .2f)]
    public float frequency = 0.02f;
    [Range(-.7f, .7f)]
    public float persistance = 0.4f;
    [HideInInspector]
    public int octaves = 4;

    float minHeight;
    float maxHeight;


    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        xOffset = Random.Range(1, 10000);
        zOffset = Random.Range(1, 10000);
    }

    void Update()
    {
        createShape();
        updateMesh();
    }

    void createShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        int index = 0;

        float xCoord = transform.position.x;
        float zCoord = transform.position.z;

        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = calculatePerlinOstaves(x + xCoord, z + zCoord, octaves, persistance);
                vertices[index] = new Vector3(x, y, z);
                if (y > maxHeight)
                    maxHeight = y;
                if (y < minHeight)
                    minHeight = y;
                index++;


            }
        }

        triangles = new int[xSize * zSize * 6];

        int verts = 0;
        int tris = 0;

        for (int j = 0; j < zSize; j++)
        {
            for (int i = 0; i < xSize; i++)
            {
                triangles[0 + tris] = 0 + verts;
                triangles[1 + tris] = xSize + 1 + verts;
                triangles[2 + tris] = 1 + verts;
                triangles[3 + tris] = 1 + verts;
                triangles[4 + tris] = xSize + 1 + verts;
                triangles[5 + tris] = xSize + 2 + verts;
                verts++;
                tris += 6;
            }
            verts++;
        }


        int index2 = 0;
        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = Mathf.InverseLerp(minHeight, maxHeight, vertices[index2].y);
                index2++;
            }
        }

    }


    float calculatePerlinOstaves(float x, float z, int octaves, float persistance)
    {
        float total = 0;
        float amplitudeX = amplitude;
        float frequencyX = frequency;
        for (int i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise(x * frequencyX + xOffset, z * frequencyX + zOffset) * amplitudeX;
            amplitudeX *= persistance;
            frequencyX *= 2;
        }
        return total;
    }


    void updateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }



    //private void OnDrawGizmos()
    //{
    //    if (vertices == null)
    //        return;
    //    for (int i = 0; i < vertices.Length; i++)
    //    {
    //        Gizmos.DrawSphere(vertices[i], .1f);
    //    }
    //}
}
