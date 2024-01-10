using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using System.Linq;

public class Waves : MonoBehaviour
{
    public int Dimensions = 10;

    public GameObject boat;

    public Vector2 offset = Vector2.zero;

    private Vector3[] vertices;

    protected MeshFilter meshFilter;
    protected Mesh mesh;
    public float UVScale;
    // Start is called before the first frame update
    void Awake()
    {
        mesh = new Mesh();

        mesh.vertices = GenerateVerts();
        mesh.triangles = GenerateTries();
        mesh.uv = GenerateUVs();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }

    private Vector2[] GenerateUVs()
    {
        var uvs = new Vector2[mesh.vertices.Length];

        for(int x = 0; x < Dimensions; x++)
            for(int z = 0; z < Dimensions; z++)
            {
                var vec = new Vector2((x / UVScale) % 2, (z / UVScale) % 2);
                uvs[x * Dimensions + z] = new Vector2(vec.x <= 1 ? vec.x : 2 - vec.x, vec.y <= 1 ? vec.y : 2 - vec.y);
            }

            return uvs;
    }

    private Vector3[] GenerateVerts()
    {
        var verts = new Vector3[(int)Mathf.Pow(Dimensions , 2)];
        Array.Fill(verts, Vector3.zero);

        return verts; 
    }

    public void SetVerts(Vector2 _offset, Vector3 boatPosition)
    {
        offset = _offset;
        var verts = mesh.vertices;
        for(int x = 0; x < Dimensions ; x++)
            for(int z = 0; z < Dimensions ; z++)
            {
                verts[x * Dimensions + z] = new Vector3((x + (offset.x * Dimensions) - (Dimensions * 3 / 4)) / 4f, 0f, (z + (offset.y * Dimensions) - (Dimensions / 2)) / 4f) + boatPosition;
            }

        mesh.vertices = verts;

        mesh.RecalculateBounds();
    }
    private int[] GenerateTries()
    {
        var tries = new int[(int)Mathf.Pow(Dimensions, 2) * 6];

        for(int x = 0; x < Dimensions; x++)
            for(int z = 0; z < Dimensions; z++)
            {
                if(x != Dimensions - 1 && z != Dimensions - 1){
                    tries[(x * Dimensions + z) * 6] = x * Dimensions + z;
                    tries[(x * Dimensions + z) * 6 + 1] = (x + 1) * Dimensions + z + 1;
                    tries[(x * Dimensions + z) * 6 + 2] = (x + 1) * Dimensions + z;
                    tries[(x * Dimensions + z) * 6 + 3] = x * Dimensions + z;
                    tries[(x * Dimensions + z) * 6 + 4] = x * Dimensions + z + 1;
                    tries[(x * Dimensions + z) * 6 + 5] = (x + 1) * Dimensions + z + 1;
                }
            }

        return tries;
    }

    void Update()
    {
       
    }
    public bool IsInBounds(Vector3 position)
    {
        int a = 0;
        int b = Dimensions * Dimensions - 1;
        if(position.x > mesh.vertices[a].x && position.x < mesh.vertices[b].x && position.z > mesh.vertices[a].z && position.z < mesh.vertices[b].z)
        {
            Debug.Log(offset);
            return true;
        }
        return false;
    }
}
