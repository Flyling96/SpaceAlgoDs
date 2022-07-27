using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MeshBuilderize
{
    public static class MeshUtility
    {
        public static Vertex[] GetVertices(this Mesh mesh)
        {
            if (mesh == null)
                return null;

            int vertexCount = mesh.vertexCount;
            Vertex[] v = new Vertex[vertexCount];

            Vector3[] positions = mesh.vertices;

            bool _hasPositions = positions != null && positions.Count() == vertexCount;

            for (int i = 0; i < vertexCount; i++)
            {
                v[i] = new Vertex();

                if (_hasPositions)
                    v[i].Position = positions[i];
            }

            return v;
        }
    }
}
