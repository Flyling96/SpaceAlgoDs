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

        public static Vertex Mix(this Vertex x, Vertex y, float weight)
        {
            float i = 1f - weight;

            Vertex v = new Vertex();

            v.Position = x.Position * i + y.Position * weight;

            return v;
        }
    }
}
