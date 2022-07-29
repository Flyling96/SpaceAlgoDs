using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Geometry;

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

            builderMesh.Builderize();

        }

    }
}
