using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridEdit))]
public class GridEditEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("GenerateGrid"))
        {
            var mono = target as GridEdit;
            mono.GerneateGrid();
        }
    }
}
