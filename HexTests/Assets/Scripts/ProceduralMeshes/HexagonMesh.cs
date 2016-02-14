//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexagonMesh : MonoBehaviour {

    public float Radius;

    // Use this for initialization
    void Awake() {
        GetComponent<MeshFilter>().sharedMesh = new Mesh();
        GenerateMesh();
    }

    void OnValidate() {
        GetComponent<MeshFilter>().sharedMesh = new Mesh();
        GenerateMesh();
    }

    void GenerateMesh() {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uv = new List<Vector2>();


        for (int i = 0; i < 6; i++) {
            int count = vertices.Count;
            vertices.Add(new Vector3(Mathf.Sin(i * 60 * Mathf.Deg2Rad) * Radius, 0, Mathf.Cos(i * 60 * Mathf.Deg2Rad) * Radius));
            uv.Add(new Vector2(0, 0));
            vertices.Add(new Vector3(Mathf.Sin((i + 1) * 60 * Mathf.Deg2Rad) * Radius, 0, Mathf.Cos((i + 1) * 60 * Mathf.Deg2Rad) * Radius));
            uv.Add(new Vector2(0, 1));
            vertices.Add(Vector3.zero);
            uv.Add(new Vector2(1, 0.5f));

            triangles.Add(count);
            triangles.Add(count + 1);
            triangles.Add(count + 2);
        }

        mesh.vertices = vertices.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();
        mesh.Optimize();
        mesh.MarkDynamic();

        //GetComponent<MeshCollider>().sharedMesh = meshFilter.mesh;
        gameObject.layer = 2;
        //GetComponent<MeshCollider>().enabled = false;
    }
}
