using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshDrawer : MonoBehaviour
{
    public MeshFilter m_meshFilter;
    public MeshRenderer m_meshRenderer;
    public Material m_material;
    public float radius = 1.0f;
    public int vertex_group = 100;

    [Range(2,8)]
    public int exponent;

    [Range(0, 6.0f)]
    public float projection;

    private int last_frame_exponent;
    private float last_frame_projection;
    void Awake()
    {
        m_meshFilter = GetComponent<MeshFilter>();
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_meshRenderer.material = m_material;
        last_frame_exponent = exponent;
        last_frame_projection = projection;
        //DrawSphere();
        DrawCalaBiYau(exponent, projection);
    }

    Vector3 GetXYZ(float theta, float phi)
    {
        float _r_theta = theta * Mathf.PI / 180.0f;
        float _r_phi = phi * Mathf.PI / 180.0f;

        float x = radius * Mathf.Sin(_r_theta) * Mathf.Cos(_r_phi);
        float y = radius * Mathf.Sin(_r_theta) * Mathf.Sin(_r_phi);
        float z = radius * Mathf.Cos(_r_theta);

        var res = new Vector3(x,y,z);
        //Debug.Log(res);
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

    Vector3 coordinate(float x, float y, float n, float k1, float k2, float a)
    {

        Complex z1 = MathX.Exp(new Complex(0, 2*Constants.π*k1/n)) * MathX.Pow(MathX.Cos(new Complex(x,y)), 2/n);
        Complex z2 = MathX.Exp(new Complex(0, 2*Constants.π*k2/n)) * MathX.Pow(MathX.Sin(new Complex(x,y)), 2/n);
        return new Vector3((float)z1.Re, (float)z2.Re, (float)z1.Im * (float)Mathf.Cos(a) + (float)z2.Im*Mathf.Sin(a));
    }

    void DrawCalaBiYau(int n, float a)
    {
        float dx = (float)Constants.π / 10;
        float dy = dx;
        List<Vector3> vertices = new List<Vector3>();
        List<int> indieces = new List<int>();
        for (int k1 = 0; k1 <= n; k1++)
        {
            for (int k2 = 0; k2 <= n; k2++)
            {
                for (float x = 0; x < (float)Constants.π/2; x += dx)
                {
                    for (float y = -(float)Constants.π/2; y < (float)Constants.π/2; y += dy)
                    {
                        float x_min = Mathf.Min(x, x+dx);
                        float x_max = Mathf.Max(x, x+dx);
                        float y_min = Mathf.Min(y, y+dy);
                        float y_max = Mathf.Max(y, y+dy);

                        Vector3 b_l = coordinate(x_min,y_min,n,k1,k2,a);
                        Vector3 b_r = coordinate(x_max,y_min,n,k1,k2,a);
                        Vector3 t_r = coordinate(x_max,y_max,n,k1,k2,a);
                        Vector3 t_l = coordinate(x_min,y_max,n,k1,k2,a);

                        Vector3 right = b_r - b_l;
                        Vector3 up = t_l - b_l;

                        Vector3 normal = Vector3.Cross(right, up);
                        normal.Normalize();

                        int idx = vertices.Count;

                        vertices.Add(b_l);
                        vertices.Add(t_l);
                        vertices.Add(t_r);
                        vertices.Add(b_r);


                        //vertices.Add(b_l + (-0.1f)*normal);
                        //vertices.Add(t_l + (-0.1f)*normal);
                        //vertices.Add(t_r + (-0.1f)*normal);
                        //vertices.Add(b_r + (-0.1f)*normal);

                        int[] cube = new int[]
                        {
                            0, 1, 2,
                            2, 3, 0,
                            //4, 7, 6,
                            //6, 5, 4,
                        };

                        foreach (int cube_i in cube)
                        {
                            indieces.Add(idx + cube_i);
                        }

                    }
                }
            }
        }

        Mesh mesh = m_meshFilter.mesh;
        mesh.Clear();

        mesh.vertices = vertices.ToArray();
        mesh.SetIndices(indieces.ToArray(), MeshTopology.Triangles, 0);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (last_frame_exponent != exponent || last_frame_projection != projection)
        {
            last_frame_exponent = exponent;
            last_frame_projection = projection;
            DrawCalaBiYau(exponent, projection);
        }

        if (m_material != m_meshRenderer.material)
        {
            m_meshRenderer.material = m_material;
        }
    }
}
