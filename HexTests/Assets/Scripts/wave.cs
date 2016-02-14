//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class wave : MonoBehaviour
{

    private MeshFilter _mesh;
    private List<Vector3> _originalVerts = new List<Vector3>();

    [System.Serializable]
    public struct Wave
    {
        [HideInInspector]
        public float Phase;
        public float PhaseIncrement;
        public float Amplitude;
        public Vector2 CoordinateWeights;
    }

    public Wave[] Waves;

    // Use this for initialization
    void Start()
    {
        _originalVerts.AddRange(GetComponent<MeshFilter>().mesh.vertices);
        _mesh = GetComponent<MeshFilter>();
    }

    void OnEnable()
    {
        for (int i = 0; i < Waves.Length; i++)
        {
            Waves[i].Phase = Time.timeSinceLevelLoad;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        for (int i = 0; i < Waves.Length; i++)
        {
            Waves[i].Phase += Waves[i].PhaseIncrement * Time.deltaTime;
        }

        List<Vector3> vertices = new List<Vector3>();

        for (int i = 0; i < _mesh.mesh.vertices.Length; i++)
        {
            int vertexIndex = i;
            Vector3 vertexPos = _mesh.mesh.vertices[vertexIndex] + transform.position;
            float height = Mathf.Sin((vertexPos.x * Waves[0].CoordinateWeights.x + vertexPos.z * Waves[0].CoordinateWeights.y) + Waves[0].Phase) * Waves[0].Amplitude;
            for (int w = 1; w < Waves.Length; w++)
            {
                height *= Mathf.Sin((vertexPos.x * Waves[w].CoordinateWeights.x + vertexPos.z * Waves[w].CoordinateWeights.y) + Waves[w].Phase) * Waves[w].Amplitude;
            }

            vertices.Add(new Vector3(vertexPos.x, _originalVerts[i].y + height, vertexPos.z) - transform.position);
        }
        _mesh.mesh.vertices = vertices.ToArray();
        _mesh.mesh.RecalculateNormals();
    }
}
