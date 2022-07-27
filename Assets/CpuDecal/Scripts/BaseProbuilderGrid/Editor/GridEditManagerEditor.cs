using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(GridEditManager))]
public class GridEditManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Save"))
        {
            var path = EditorUtility.OpenFilePanel("Save ElementalGrid", "Assets", "bytes");
            if(!string.IsNullOrEmpty(path))
            {
                using(var file = File.Open(path, FileMode.OpenOrCreate))
                {
                    var writer = new BinaryWriter(file);
                }
            }

        }
    }
}
