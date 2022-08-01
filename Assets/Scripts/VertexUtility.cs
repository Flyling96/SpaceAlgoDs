using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Geometry
{
    public static class VertexUtility
    {
        public static Vertex[] GetVertices(Mesh mesh)
        {
            if (mesh == null)
                return null;

            int vertexCount = mesh.vertexCount;
            Vertex[] v = new Vertex[vertexCount];

            Vector3[] positions = mesh.vertices;

            for (int i = 0; i < vertexCount; i++)
            {
                v[i] = new Vertex();
                v[i].Position = positions[i];
            }

            return v;
        }

        public static Vertex TransformVertex(this Transform transform, Vertex vertex)
        {
            var v = new Vertex();
            v.Position = transform.TransformPoint(vertex.Position);
            return v;
        }

        public static AABB TransformAABB(this Transform transform, AABB aabb)
        {
            AABB res = new AABB();
            res.m_Min = transform.TransformPoint(aabb.m_Min);
            res.m_Max = transform.TransformPoint(aabb.m_Max);
            return res;
        }
    }
}
