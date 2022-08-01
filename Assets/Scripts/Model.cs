﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Geometry
{
    [Serializable]
    public class Model
    {
        [HideInInspector]
        public Vertex[] m_Vertices = new Vertex[0];

        [HideInInspector]
        public Face[] m_Faces = new Face[0];

        public AABB m_AABB;

        public void Clear()
        {
            m_Vertices = new Vertex[0];
            m_Faces = new Face[0];
        }

        public Model(Mesh mesh, Transform transform)
        {
            if (transform == null)
                throw new ArgumentNullException("transform");

            var sourceVertices = VertexUtility.GetVertices(mesh).Select(x => transform.TransformVertex(x)).ToArray();

            List<Vertex> splitVertices = new List<Vertex>();
            List<Face> faces = new List<Face>();

            int vertexIndex = 0;

            for (int submeshIndex = 0; submeshIndex < mesh.subMeshCount; submeshIndex++)
            {
                switch (mesh.GetTopology(submeshIndex))
                {
                    case MeshTopology.Triangles:
                        {
                            int[] indexes = mesh.GetIndices(submeshIndex);

                            for (int tri = 0; tri < indexes.Length; tri += 3)
                            {
                                faces.Add(new Face(new int[]
                                {
                                    vertexIndex, vertexIndex + 1, vertexIndex + 2
                                }));

                                splitVertices.Add(sourceVertices[indexes[tri]]);
                                splitVertices.Add(sourceVertices[indexes[tri + 1]]);
                                splitVertices.Add(sourceVertices[indexes[tri + 2]]);

                                vertexIndex += 3;
                            }
                        }
                        break;

                    case MeshTopology.Quads:
                        {
                            int[] indexes = mesh.GetIndices(submeshIndex);

                            for (int quad = 0; quad < indexes.Length; quad += 4)
                            {
                                faces.Add(new Face(new int[]
                                    {
                                    vertexIndex, vertexIndex + 1, vertexIndex + 2,
                                    vertexIndex + 2, vertexIndex + 3, vertexIndex + 0
                                    }));

                                splitVertices.Add(sourceVertices[indexes[quad]]);
                                splitVertices.Add(sourceVertices[indexes[quad + 1]]);
                                splitVertices.Add(sourceVertices[indexes[quad + 2]]);
                                splitVertices.Add(sourceVertices[indexes[quad + 3]]);

                                vertexIndex += 4;
                            }
                        }
                        break;

                    default:
                        throw new NotSupportedException("Only supports importing triangle and quad meshes.");
                }
            }

            m_Vertices = splitVertices.ToArray();
            m_Faces = faces.ToArray();
            RefreshAABB();
        }

        private void RefreshAABB()
        {
            m_AABB.m_Max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            m_AABB.m_Min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

            for (int i = 0; i < m_Vertices.Length; i++)
            {
                var vertex = m_Vertices[i];
                m_AABB.m_Max = Vector3.Max(m_AABB.m_Max,vertex.Position);
                m_AABB.m_Min = Vector3.Min(m_AABB.m_Min, vertex.Position);
            }
        }
    }
}
