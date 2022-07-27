using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public partial class GridItem
{
    public Vector2Int m_PanelCoord;

    public float m_Height;

    public bool m_IsEdge;

    public GridItem(Vector2Int panelCoord,float height)
    {
        m_PanelCoord = panelCoord;
        m_Height = height;
    }


    public override bool Equals(object obj)
    {
        GridItem other = obj as GridItem;
        if(other.m_PanelCoord == m_PanelCoord && other.m_Height == m_Height)
        {
            return true;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
#if UNITY_EDITOR
public partial class GridItem
{
    public GameObject m_Debugger;

    public void UpdateDebug(Mesh mesh, Material material, Transform parent, float gridScale)
    {
        if (m_Debugger == null)
        {
            m_Debugger = new GameObject($"Grid_{m_PanelCoord.x}_{m_PanelCoord.y}");
            m_Debugger.AddComponent<MeshFilter>().mesh = mesh;
            m_Debugger.AddComponent<MeshRenderer>().material = material;
            m_Debugger.transform.SetParent(parent);
        }

        m_Debugger.transform.localScale = Vector3.one * gridScale;
        m_Debugger.transform.localPosition = new Vector3(m_PanelCoord.x * gridScale, m_Height, m_PanelCoord.y * gridScale);
    }
}

#endif
