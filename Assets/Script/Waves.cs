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

    public bool automaticNormals = true;
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

        for(int x = 0; x < Dimensions + 1; x++)
            for(int z = 0; z < Dimensions + 1; z++)
            {
                var vec = new Vector2((x / UVScale) % 2, (z / UVScale) % 2);
                uvs[x * (Dimensions + 1) + z] = new Vector2(vec.x <= 1 ? vec.x : 2 - vec.x, vec.y <= 1 ? vec.y : 2 - vec.y);
            }

            return uvs;
    }

    private Vector3[] GenerateVerts()
    {
        var verts = new Vector3[(int)Mathf.Pow(Dimensions + 1, 2)];
        Array.Fill(verts, Vector3.zero);

        return verts; 
    }
    private int[] GenerateTries()
    {
        var tries = new int[(int)Mathf.Pow(Dimensions, 2) * 6];

        for(int x = 0; x < Dimensions; x++)
            for(int z = 0; z < Dimensions; z++)
            {
                if(x != 0 && z != 0 && x != Dimensions - 1 && z != Dimensions - 1)
                {
                    tries[(x * (Dimensions + 1) + z) * 6] = x * (Dimensions + 1) + z;
                    tries[(x * (Dimensions + 1) + z) * 6 + 1] = (x + 1) * (Dimensions + 1) + z + 1;
                    tries[(x * (Dimensions + 1) + z) * 6 + 2] = (x + 1) * (Dimensions + 1) + z;
                    tries[(x * (Dimensions + 1) + z) * 6 + 3] = x * (Dimensions + 1) + z;
                    tries[(x * (Dimensions + 1) + z) * 6 + 4] = x * (Dimensions + 1) + z + 1;
                    tries[(x * (Dimensions + 1) + z) * 6 + 5] = (x + 1) * (Dimensions + 1) + z + 1;
                }
            }

        return tries;
    }

    private Vector3[] SetNormals(Vector3[] verts)
    {
        Vector3[] normals = new Vector3[(int)Mathf.Pow(Dimensions + 1, 2)];

        for(int x = 0; x < Dimensions + 1; x++)
            for(int z = 0; z < Dimensions + 1; z++)
            {
                if(x != 0 && z != 0 && x != Dimensions && z != Dimensions)
                {
                    normals[x*(Dimensions + 1) + z] = Vector3.zero;
                    normals[x*(Dimensions + 1) + z] -= Vector3.Cross((verts[x*(Dimensions + 1) + z - Dimensions - 2] - verts[x* (Dimensions + 1) + z]), (verts[x*(Dimensions + 1) + z - 1] - verts[x* (Dimensions + 1) + z]));
                    normals[x*(Dimensions + 1) + z] -= Vector3.Cross((verts[x*(Dimensions + 1) + z - 1] - verts[x* (Dimensions + 1) + z]), (verts[x*(Dimensions + 1) + z + Dimensions] - verts[x* (Dimensions + 1) + z]));
                    normals[x*(Dimensions + 1) + z] -= Vector3.Cross((verts[x*(Dimensions + 1) + z + Dimensions] - verts[x* (Dimensions + 1) + z]), (verts[x*(Dimensions + 1) + z + Dimensions + 1] - verts[x* (Dimensions + 1) + z]));
                    normals[x*(Dimensions + 1) + z] -= Vector3.Cross((verts[x*(Dimensions + 1) + z + Dimensions + 1] - verts[x* (Dimensions + 1) + z]), (verts[x*(Dimensions + 1) + z + Dimensions + 2] - verts[x* (Dimensions + 1) + z]));
                    normals[x*(Dimensions + 1) + z] -= Vector3.Cross((verts[x*(Dimensions + 1) + z + Dimensions + 2] - verts[x* (Dimensions + 1) + z]), (verts[x*(Dimensions + 1) + z + 1] - verts[x* (Dimensions + 1) + z]));
                    normals[x*(Dimensions + 1) + z] -= Vector3.Cross((verts[x*(Dimensions + 1) + z + 1] - verts[x* (Dimensions + 1) + z]), (verts[x*(Dimensions + 1) + z - Dimensions] - verts[x* (Dimensions + 1) + z]));
                    normals[x*(Dimensions + 1) + z] -= Vector3.Cross((verts[x*(Dimensions + 1) + z - Dimensions] - verts[x* (Dimensions + 1) + z]), (verts[x*(Dimensions + 1) + z - Dimensions - 1] - verts[x* (Dimensions + 1) + z]));
                    normals[x*(Dimensions + 1) + z] -= Vector3.Cross((verts[x*(Dimensions + 1) + z - Dimensions - 1] - verts[x* (Dimensions + 1) + z]), (verts[x*(Dimensions + 1) + z - Dimensions - 2] - verts[x* (Dimensions + 1) + z]));
                    normals[x*(Dimensions + 1) + z].Normalize();
                }
                else
                   normals[x*(Dimensions + 1) + z] = Vector3.zero; 
            }
        
        return normals;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void UpdateMesh(List<Octave> octaves)
    {
        if(mesh.vertices is null && mesh.vertices.Length == 0)
            return;
        var verts = mesh.vertices;
        for(int x = 0; x < Dimensions + 1; x++)
            for(int z = 0; z < Dimensions + 1; z++)
            {
                var y = 0f;
                foreach(Octave o in octaves)
                {
                    var dir = o.direction.normalized;

                    y += Mathf.Cos(verts[x* (Dimensions + 1) + z].x 
                                    * dir.x + verts[x * (Dimensions + 1) + z].z 
                                    * dir.y + Time.time * o.speed) 
                                    * o.scale;
                }
                
                verts[x * (Dimensions + 1) + z] = new Vector3(x + (offset.x * (Dimensions - 2)), y, z + (offset.y * (Dimensions - 2)));
            }

        mesh.vertices = verts;

        var nor = SetNormals(verts);
        if(!automaticNormals)
            mesh.SetNormals(nor);
        else
            mesh.RecalculateNormals();

        mesh.RecalculateBounds();
    }


    public bool IsInBounds(Vector3 position)
    {
        int a = Dimensions + 2;
        int b = Dimensions * Dimensions - 2;
        if(position.x > mesh.vertices[a].x && position.x < mesh.vertices[b].x && position.z > mesh.vertices[a].z && position.z < mesh.vertices[b].z)
            return true;
        return false;
    }

    public float GetHeight(Vector3 position)
    {

        List<Vector3> verts = mesh.vertices.OrderByDescending(v => Vector3.Magnitude(v - position)).ToList();

        float y = 0f;

        for(int i = 0; i < 9; i++)
            y += verts[i].y;
        
        return y;

    }
}
