using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace MeshBuilderize
{
    public class MeshBuilderizeHelper
    {
        [MenuItem("GameObject/MeshBuilderize",false,0)]
        public static void MeshBuilderize()
        {
            var selection = Selection.activeGameObject;
            if(selection == null)
            {
                return;
            }

            var meshFilters = selection.GetComponentsInChildren<MeshFilter>();
            if(meshFilters == null)
            {
                return;
            }

            foreach (var meshFilter in meshFilters)
            {
                BuildMesh(meshFilter);
            }
        }

        private static void BuildMesh(MeshFilter meshFilter)
        {
            var mesh = meshFilter.mesh;
            if (mesh == null)
            {
                return;
            }

            var builderMesh = meshFilter.GetComponent<BuilderMesh>();
            if (builderMesh == null)
            {
                builderMesh = Undo.AddComponent<BuilderMesh>(meshFilter.gameObject);
            }

            var sourceVertices = mesh.GetVertices();
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

            builderMesh.Init(splitVertices, faces);


        }

    }
}
