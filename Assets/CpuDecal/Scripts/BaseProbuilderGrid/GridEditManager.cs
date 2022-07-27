#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.ProBuilder;
using System.IO;

[ExecuteInEditMode]
public class GridEditManager : MonoBehaviour
{
    private static GridEditManager m_Instance = null;

    public static GridEditManager Instance
    {
        get
        {
            if(m_Instance == null)
            {
                var go = GameObject.Find(nameof(GridEditManager));
                if (go == null)
                {
                    go = new GameObject(nameof(GridEditManager));
                    m_Instance = go.AddComponent<GridEditManager>();
                }
                else
                {
                    m_Instance = go.GetComponent<GridEditManager>();
                }
            }

            return m_Instance;

        }
    }

    public ProBuilderMesh m_SceneMesh = null;

    public Material Material = null;

    public void SerializeElementalGrid(string path)
    {
        var grids = transform.GetComponentInChildren<GridEdit>();
        using (var file = File.Open(path, FileMode.OpenOrCreate))
        {
            var writer = new BinaryWriter(file);
            
        }
    }
}

#endif
