using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PlaneMesh : MonoBehaviour
{

    private Mesh _mesh;
    private List<Vector3> _vertices;
    private List<int> _indices;
    private List<Vector2> _uvs;

    public List<Vector3> Vertices
    {
        get { return _vertices; }
        set { _vertices = value; }
    }

    [System.Serializable]
    public struct POINT
    {
        public int x;
        public int y;
    }

    [System.Serializable]
    public enum SmoothType
    {
        SMOOTH,
        HARD
    };

    public Vector2 Dimensions;
    public SmoothType Type = SmoothType.HARD;
    public POINT Segments;


    // Use this for initialization
    void Start()
    {
        Rebuild();
    }

    // Update is called once per frame
    void Update()
    {
        Segments.x = Mathf.Clamp(Segments.x, 1, 512);
        Segments.y = Mathf.Clamp(Segments.y, 1, 512);
    }

    public void UpdateMeshFilter()
    {
        _mesh.vertices = _vertices.ToArray();
        _mesh.RecalculateNormals();
    }

    public void Rebuild()
    {
        switch (Type)
        {
            case SmoothType.SMOOTH:
                RebuildSmooth();
                break;
            case SmoothType.HARD:
                RebuildHard();
                break;
            default:
                break;
        }
    }

    private void RebuildSmooth()
    {
        _vertices = new List<Vector3>((Segments.x + 1) * (Segments.y + 1));
        _uvs = new List<Vector2>((Segments.x + 1) * (Segments.y + 1));

        for (int x = 0; x <= Segments.x; x++)
        {
            for (int y = 0; y <= Segments.y; y++)
            {
                Vector3 vertex = Vector3.one;

                vertex.x *= (Dimensions.x / -2) + (Dimensions.x / Segments.x) * x;
                vertex.y = 0;
                vertex.z *= (Dimensions.y / -2) + (Dimensions.y / Segments.y) * y;

                _uvs.Add(new Vector2(x * (1.0f / Segments.x), y * (1.0f / Segments.y)));
                _vertices.Add(vertex);
            }
        }

        _indices = new List<int>(Segments.x * Segments.y * 6);
        for (int x = 0; x < Segments.x; x++)
        {
            for (int y = 0; y < Segments.y; y++)
            {
                int baseVertex = y + x * (Segments.y + 1);

                _indices.Add(baseVertex);
                _indices.Add(baseVertex + 1);
                _indices.Add(baseVertex + Segments.y + 2);
                _indices.Add(baseVertex);
                _indices.Add(baseVertex + Segments.y + 2);
                _indices.Add(baseVertex + Segments.y + 1);
            }
        }

        List<Vector3> normals = new List<Vector3>((Segments.x + 1) * (Segments.y + 1));
        for (int i = 0; i < (Segments.x + 1) * (Segments.y + 1); i++)
        {
            normals.Add(Vector3.up);
        }

        _mesh = new Mesh();
        _mesh.vertices = _vertices.ToArray();
        _mesh.triangles = _indices.ToArray();
        _mesh.uv = _uvs.ToArray();
        _mesh.normals = normals.ToArray();

        GetComponent<MeshFilter>().mesh = _mesh;

    }

    private void RebuildHard()
    {
        _vertices = new List<Vector3>((Segments.x + 1) * (Segments.y + 1));
        _uvs = new List<Vector2>((Segments.x + 1) * (Segments.y + 1));

        for (int x = 0; x < Segments.x; x++)
        {
            for (int y = 0; y < Segments.y; y++)
            {
                int newX = x;
                int newY = y;

                //lBot
                Vector3 vertex = Vector3.one;

                vertex.x *= (Dimensions.x / -2) + (Dimensions.x / Segments.x) * newX;
                vertex.y = 0;
                vertex.z *= (Dimensions.y / -2) + (Dimensions.y / Segments.y) * newY;

                _uvs.Add(new Vector2(newX * (1.0f / Segments.x), newY * (1.0f / Segments.y)));
                _vertices.Add(vertex);

                //lTop
                vertex = Vector3.one;

                ++newY;

                vertex.x *= (Dimensions.x / -2) + (Dimensions.x / Segments.x) * newX;
                vertex.y = 0;
                vertex.z *= (Dimensions.y / -2) + (Dimensions.y / Segments.y) * newY;

                _uvs.Add(new Vector2(newX * (1.0f / Segments.x), newY * (1.0f / Segments.y)));
                _vertices.Add(vertex);

                //rTop
                vertex = Vector3.one;

                ++newX;

                vertex.x *= (Dimensions.x / -2) + (Dimensions.x / Segments.x) * newX;
                vertex.y = 0;
                vertex.z *= (Dimensions.y / -2) + (Dimensions.y / Segments.y) * newY;

                _uvs.Add(new Vector2(newX * (1.0f / Segments.x), newY * (1.0f / Segments.y)));
                _vertices.Add(vertex);

                //lBot
                vertex = Vector3.one;

                --newX;
                --newY;

                vertex.x *= (Dimensions.x / -2) + (Dimensions.x / Segments.x) * newX;
                vertex.y = 0;
                vertex.z *= (Dimensions.y / -2) + (Dimensions.y / Segments.y) * newY;

                _uvs.Add(new Vector2(newX * (1.0f / Segments.x), newY * (1.0f / Segments.y)));
                _vertices.Add(vertex);

                //rTop
                vertex = Vector3.one;

                ++newX;
                ++newY;

                vertex.x *= (Dimensions.x / -2) + (Dimensions.x / Segments.x) * newX;
                vertex.y = 0;
                vertex.z *= (Dimensions.y / -2) + (Dimensions.y / Segments.y) * newY;

                _uvs.Add(new Vector2(newX * (1.0f / Segments.x), newY * (1.0f / Segments.y)));
                _vertices.Add(vertex);


                //rBot
                vertex = Vector3.one;

                --newY;

                vertex.x *= (Dimensions.x / -2) + (Dimensions.x / Segments.x) * newX;
                vertex.y = 0;
                vertex.z *= (Dimensions.y / -2) + (Dimensions.y / Segments.y) * newY;

                _uvs.Add(new Vector2(newX * (1.0f / Segments.x), newY * (1.0f / Segments.y)));
                _vertices.Add(vertex);

            }
        }

        _indices = new List<int>(Segments.x * Segments.y * 4 * 6);
        for (int x = 0; x < Segments.x; x++)
        {
            for (int y = 0; y < Segments.y; y++)
            {
                int baseVertex = (y + x * (Segments.y)) * 6;

                _indices.Add(baseVertex);
                _indices.Add(baseVertex + 1);
                _indices.Add(baseVertex + 2);
                _indices.Add(baseVertex + 3);
                _indices.Add(baseVertex + 4);
                _indices.Add(baseVertex + 5);
            }
        }

        _mesh = new Mesh();
        _mesh.vertices = _vertices.ToArray();
        _mesh.triangles = _indices.ToArray();
        _mesh.uv = _uvs.ToArray();
        _mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = _mesh;


    }

}