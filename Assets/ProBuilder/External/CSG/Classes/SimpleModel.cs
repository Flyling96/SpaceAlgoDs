using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.ProBuilder.Csg
{
    public class SimpleModel
    {
        List<Vector3> m_Vertices;
        List<int> m_Indices;

        internal SimpleModel(List<Polygon> polygons)
        {
            m_Vertices = new List<Vector3>();
            m_Indices = new List<int>();

            int p = 0;

            for (int i = 0; i < polygons.Count; i++)
            {
                Polygon poly = polygons[i];

                for (int j = 2; j < poly.vertices.Count; j++)
                {
                    var pos = poly.vertices[0].position;
                    var index = IndexOfVertices(pos);
                    if(index == -1)
                    {
                        m_Vertices.Add(pos);
                        m_Indices.Add(p++);
                    }
                    else
                    {
                        m_Indices.Add(index);
                    }

                    pos = poly.vertices[j - 1].position;
                    index = IndexOfVertices(pos);
                    if (index == -1)
                    {
                        m_Vertices.Add(pos);
                        m_Indices.Add(p++);
                    }
                    else
                    {
                        m_Indices.Add(index);
                    }

                    pos = poly.vertices[j].position;
                    index = IndexOfVertices(pos);
                    if (index == -1)
                    {
                        m_Vertices.Add(pos);
                        m_Indices.Add(p++);
                    }
                    else
                    {
                        m_Indices.Add(index);
                    }
                }
            }
        }

        public int IndexOfVertices(Vector3 position)
        {
            for (int i = 0; i < m_Vertices.Count; i++)
            {
                var vertex = m_Vertices[i];
                if(Vector3.Distance(vertex,position) <= 0.01f)
                {
                    return i;
                }
            }

            return -1;
        }


        public Mesh mesh
        {
            get { return (Mesh)this; }
        }

        public static explicit operator Mesh(SimpleModel model)
        {
            var mesh = new Mesh();
            mesh.vertices = model.m_Vertices.ToArray();
            mesh.SetIndices(model.m_Indices, MeshTopology.Triangles, 0);
            return mesh;
        }
    }
}
