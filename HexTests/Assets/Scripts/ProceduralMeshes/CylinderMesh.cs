//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]

public class CylinderMesh : MonoBehaviour {

    public float Radius = 1.0f;
    public float Height = 1.0f;
    public int Sides = 6;


    // Use this for initialization
    void Awake() {
#if UNITY_EDITOR
        GetComponent<MeshFilter>().sharedMesh = new Mesh();
#else
        GetComponent<MeshFilter>().mesh = new Mesh();
#endif
        GenerateMesh();
    }

    void OnValidate() {
        Sides = Mathf.Clamp(Sides, 3, 128);
        Radius = Mathf.Clamp(Radius, 0, Mathf.Infinity);

        GenerateMesh();
    }

    void GenerateMesh() {
#if UNITY_EDITOR
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
#else
        Mesh mesh = GetComponent<MeshFilter>().mesh;
#endif

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uv = new List<Vector2>();

        float angleIncrement = 360.0f / Sides;
        angleIncrement *= Mathf.Deg2Rad;

        for (int i = 0; i < Sides; i++) {

            int count = vertices.Count;
            vertices.Add(new Vector3(Mathf.Sin(i * angleIncrement) * Radius, 0, Mathf.Cos(i * angleIncrement) * Radius));
            uv.Add(new Vector2(1, 0));
            vertices.Add(new Vector3(Mathf.Sin((i + 1) * angleIncrement) * Radius, 0, Mathf.Cos((i + 1) * angleIncrement) * Radius));
            uv.Add(new Vector2(0, 0));
            vertices.Add(new Vector3(Mathf.Sin((i + 1) * angleIncrement) * Radius, Height, Mathf.Cos((i + 1) * angleIncrement) * Radius));
            uv.Add(new Vector2(0, 1));
            vertices.Add(new Vector3(Mathf.Sin(i * angleIncrement) * Radius, Height, Mathf.Cos(i * angleIncrement) * Radius));
            uv.Add(new Vector2(1, 1));


            triangles.Add(count);
            triangles.Add(count + 1);
            triangles.Add(count + 2);
            triangles.Add(count);
            triangles.Add(count + 2);
            triangles.Add(count + 3);
        }

        //Debug.Log(string.Format("vertices: {0}, uvs: {1}" , vertices.Count, uv.Count));

        if (mesh != null) {
            mesh.vertices = vertices.ToArray();
            mesh.uv = uv.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
            mesh.Optimize();
        }
    }

}
