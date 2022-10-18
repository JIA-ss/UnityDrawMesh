using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshDrawer : MonoBehaviour
{
    public MeshFilter m_meshFilter;
    public MeshRenderer m_meshRenderer;
    public Material m_material;
    public float radius = 1.0f;
    public int vertex_group = 100;
    void Awake()
    {
        m_meshFilter = GetComponent<MeshFilter>();
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_meshRenderer.material = m_material;
        DrawSphere();
    }

    Vector3 GetXYZ(float theta, float phi)
    {
        float _r_theta = theta * Mathf.PI / 180.0f;
        float _r_phi = phi * Mathf.PI / 180.0f;

        float x = radius * Mathf.Sin(_r_theta) * Mathf.Cos(_r_phi);
        float y = radius * Mathf.Sin(_r_theta) * Mathf.Sin(_r_phi);
        float z = radius * Mathf.Cos(_r_theta);

        var res = new Vector3(x,y,z);
        Debug.Log(res);
        return res;
    }
    void DrawSphere()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> indicies = new List<int>();
        float threthod = 360.0f/vertex_group;
        for (float theta = 0; theta < 360; theta += threthod)
        {
            for (float phi = 0; phi < 360; phi += threthod)
            {
                Vector3 b_r = GetXYZ(theta, phi);
                Vector3 b_l = GetXYZ(theta + threthod, phi);
                Vector3 t_r = GetXYZ(theta, phi - threthod);
                Vector3 t_l = GetXYZ(theta + threthod, phi - threthod);

                int idx = vertices.Count;
                vertices.Add(b_l);
                vertices.Add(t_l);
                vertices.Add(t_r);
                vertices.Add(b_r);

                indicies.Add(idx);
                indicies.Add(idx + 1);
                indicies.Add(idx + 2);

                indicies.Add(idx + 2);
                indicies.Add(idx + 3);
                indicies.Add(idx);
            }
        }

        Mesh mesh = m_meshFilter.mesh;
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.SetIndices(indicies.ToArray(), MeshTopology.Triangles, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
