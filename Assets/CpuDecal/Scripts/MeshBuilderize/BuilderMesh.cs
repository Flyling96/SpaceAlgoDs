using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshBuilderize
{
    public class BuilderMesh : MonoBehaviour
    {
        [HideInInspector]
        public Vector3[] m_Vertices = new Vector3[0];

        [HideInInspector]
        public Face[] m_Faces = new Face[0];

        public void Init(List<Vertex> vertices, List<Face> faces)
        {
            m_Vertices = new Vector3[vertices.Count];
            for (int i = 0; i < vertices.Count; i++)
            {
                m_Vertices[i] = vertices[i].Position;
            }

            m_Faces = faces.ToArray();
        }

        public void Clear()
        {
            m_Vertices = new Vector3[0];
            m_Faces = new Face[0];
        }
    }
}
