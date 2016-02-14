using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlaneMesh))]
public class wave : MonoBehaviour
{

    private PlaneMesh _planeMesh;

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
        _planeMesh = GetComponent<PlaneMesh>();
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


        for (int i = 0; i < _planeMesh.Vertices.Count; i++)
        {
            int vertexIndex = i;
            Vector3 vertexPos = _planeMesh.Vertices[vertexIndex] + transform.position;
            float height = Mathf.Sin((vertexPos.x * Waves[0].CoordinateWeights.x + vertexPos.z * Waves[0].CoordinateWeights.y) + Waves[0].Phase) * Waves[0].Amplitude;
            for (int w = 1; w < Waves.Length; w++)
            {
                height *= Mathf.Sin((vertexPos.x * Waves[w].CoordinateWeights.x + vertexPos.z * Waves[w].CoordinateWeights.y) + Waves[w].Phase) * Waves[w].Amplitude;
            }
            _planeMesh.Vertices[vertexIndex] = new Vector3(vertexPos.x, height, vertexPos.z) - transform.position;
        }

        _planeMesh.UpdateMeshFilter();
    }
}
