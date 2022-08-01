﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;

namespace MeshBuilderize
{
    [CustomEditor(typeof(BuilderMeshManager))]
    public class BuilderMeshManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            var debugDepth = serializedObject.FindProperty("m_DebugDepth");
            if(debugDepth != null)
            {
                debugDepth.intValue = EditorGUILayout.IntSlider("DebugDepth", debugDepth.intValue, -1, 10);
            }
            if(EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

            var manager = target as BuilderMeshManager;
            if (GUILayout.Button("Builderize"))
            {
                manager.Builderize();
            }
            else if (GUILayout.Button("BuildBVHTree"))
            {
                manager.BuildBVHTree();
                var stage = PrefabStageUtility.GetCurrentPrefabStage();
                if (stage == null)
                {
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                }
                else
                {
                    EditorSceneManager.MarkSceneDirty(stage.scene);
                }
            }
        }
    }
}
