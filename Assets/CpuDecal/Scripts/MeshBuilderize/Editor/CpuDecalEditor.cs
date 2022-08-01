using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MeshBuilderize
{
    [CustomEditor(typeof(CpuDecal))]
    public class CpuDecalEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var cpuDecal = target as CpuDecal;
            if(GUILayout.Button("CollideBVH"))
            {
                cpuDecal.CollideBVH();
            }
        }
    }
}
