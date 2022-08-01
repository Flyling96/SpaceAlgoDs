using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MeshBuilderize
{
    [CustomEditor(typeof(BuilderMesh))]
    public class BuilderMeshEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var builderMesh = target as BuilderMesh;
            if(GUILayout.Button("Builderize"))
            {
                builderMesh.Builderize();
            }
        }
    }
}
