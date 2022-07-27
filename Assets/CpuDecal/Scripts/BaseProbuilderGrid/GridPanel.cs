using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Triangle
{
    public Vector3[] m_Vertices;

    public Vector2[] m_2dVertices;

    public Triangle(Vector3 a, Vector3 b, Vector3 c)
    {
        m_Vertices = new Vector3[3] { a, b, c };

        m_2dVertices = new Vector2[3] { new Vector2(a.x, a.z), new Vector2(b.x, b.z), new Vector2(c.x, c.z) };
    }

    public bool IsInside(Vector2 coord)
    {
        var ab = m_2dVertices[1] - m_2dVertices[0];
        var bc = m_2dVertices[2] - m_2dVertices[1];
        var ca = m_2dVertices[0] - m_2dVertices[2];

        var ap = coord - m_2dVertices[0];
        var bp = coord - m_2dVertices[1];
        var cp = coord - m_2dVertices[2];

        var a = ab.Cross(ap);
        var b = bc.Cross(bp);
        var c = ca.Cross(cp);

        return (a >= 0 && b >= 0 && c >= 0) || (a <= 0 && b <= 0 && c <= 0);
    }

    public float CaculateHeight(Vector2 coord)
    {
        //重心坐标求插值  
        //Error 重心坐标不具备投影不变性
        //var pa = m_2dVertices[0] - coord;
        //var pb = m_2dVertices[1] - coord;
        //var pc = m_2dVertices[2] - coord;

        //var ab = m_2dVertices[1] - m_2dVertices[0];
        //var ac = m_2dVertices[2] - m_2dVertices[1];

        //var sum = Mathf.Abs(ab.Cross(ac));

        //var a = Mathf.Abs(pb.Cross(pc));
        //var b = Mathf.Abs(pa.Cross(pc));
        //var c = Mathf.Abs(pa.Cross(pb));

        //return m_Vertices[0].y * a + m_Vertices[1].y * b + m_Vertices[2].y * c;

        //平面方程求解
        var x0 = m_Vertices[0].x;var y0 = m_Vertices[0].y;var z0 = m_Vertices[0].z;
        //var x1 = m_Vertices[1].x;var y1 = m_Vertices[1].y;var z1 = m_Vertices[1].z;
        //var x2 = m_Vertices[2].x;var y2 = m_Vertices[2].y;var z2 = m_Vertices[2].z;

        //公式错误？
        //var a = (y2 - y0) * (z2 - z0) - (z1 - z0) * (y2 - y0);
        //var b = (x2 - x0) * (z1 - z0) - (x1 - x0) * (z2 - z0);
        //var c = (x1 - x0) * (y2 - y0) - (x2 - x0) * (y1 - y0);
        //var d = -(a * x0 + b * y0 + c * z0);
        //var res = -(a * coord.x + c * coord.y + d) / b;

        //为什么法线对应一般式的A、B、C?
        var ab = m_Vertices[1] - m_Vertices[0];
        var ac = m_Vertices[2] - m_Vertices[0];
        var normal = Vector3.Cross(ab, ac);
        var d = -(normal.x * x0 + normal.y * y0 + normal.z * z0);
        var res = -(normal.x * coord.x + normal.z * coord.y + d) / normal.y;

        return res;

    }
}

public struct TriangleMesh
{
    public List<Triangle> m_Triangles;

    public Vector2 m_CorrdMin;

    public Vector2 m_CoordMax;

    public TriangleMesh(Mesh mesh)
    {
        m_CorrdMin = new Vector2(float.MaxValue, float.MaxValue);
        m_CoordMax = new Vector2(float.MinValue, float.MinValue);
        m_Triangles = new List<Triangle>();
        var vertices = mesh.vertices;
        var indices = mesh.triangles;

        for (int i = 0; i < indices.Length; i+= 3 )
        {
            var index0 = indices[i];
            var index1 = indices[i + 1];
            var index2 = indices[i + 2];

            var v0 = vertices[index0];
            var v1 = vertices[index1];
            var v2 = vertices[index2];
            m_Triangles.Add(new Triangle(v0, v1, v2));
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            var vertex = vertices[i];
            if(vertex.x < m_CorrdMin.x)
            {
                m_CorrdMin.x = vertex.x;
            }
            else if(vertex.x > m_CoordMax.x)
            {
                m_CoordMax.x = vertex.x;
            }

            if(vertex.z < m_CorrdMin.y)
            {
                m_CorrdMin.y = vertex.z;
            }
            else if(vertex.z> m_CoordMax.y)
            {
                m_CoordMax.y = vertex.z;
            }
        }
    }

    public bool IsInside(Vector2 coord,out float height)
    {
        height = 0;

        for (int i = 0; i < m_Triangles.Count; i++)
        {
            var triangle = m_Triangles[i];
            if(triangle.IsInside(coord))
            {
                height = triangle.CaculateHeight(coord);
                return true;
            }
        }

        return false;
    }

    public bool IsInside(Vector2 coord)
    {
        for (int i = 0; i < m_Triangles.Count; i++)
        {
            var triangle = m_Triangles[i];
            if (triangle.IsInside(coord))
            {
                return true;
            }
        }

        return false;
    }

    public bool IsInsideAABB(Vector2 coord)
    {
        return coord.x <= m_CoordMax.x && coord.y <= m_CoordMax.y && coord.x >= m_CorrdMin.x && coord.y >= m_CorrdMin.y;
    }
}

[System.Serializable]
public class GridPanel
{
    [HideInInspector]
    public Vector3 m_Position;

    public float m_GridSize = 0.5f;

    public HashSet<GridItem> m_Grids = new HashSet<GridItem>();

    public void GenerateGrid(Mesh mesh, Vector3 position)
    {
        m_Position = position;
        m_Grids.Clear();
        TriangleMesh triangleMesh = new TriangleMesh(mesh);
        var offsets = new Vector2Int[4] { new Vector2Int(0, 1), new Vector2Int(0, -1),
                new Vector2Int(-1, 0), new Vector2Int(1, 0) };

        HashSet<Vector2Int> findNext = new HashSet<Vector2Int>();
        float height = 0;
        var startCoord = Vector2Int.zero;
        if (triangleMesh.IsInside(startCoord, out height))
        {
            m_Grids.Add(new GridItem(startCoord, height));
        }

        HashSet<Vector2Int> hashFind = new HashSet<Vector2Int>();
        hashFind.Add(startCoord);
        findNext.Add(startCoord);

        Vector2Int nextCoord;
        Vector2 nextPos;
        while (findNext.Count != 0)
        {
            HashSet<Vector2Int> nextSet = new HashSet<Vector2Int>();
            foreach(var coord in findNext)
            {
                for (int i = 0; i < 4; i++)
                {
                    nextCoord = coord + offsets[i];
                    nextPos = new Vector2(nextCoord.x * m_GridSize,nextCoord.y * m_GridSize);
                    if (!hashFind.Contains(nextCoord))
                    {
                        if (triangleMesh.IsInside(nextPos, out height))
                        {
                            m_Grids.Add(new GridItem(nextCoord, height));
                        }
                        if (triangleMesh.IsInsideAABB(nextPos))
                        {
                            nextSet.Add(nextCoord);
                        }
                    }
                }
            }
            findNext = nextSet;
            foreach (var next in nextSet)
            {
                hashFind.Add(next);
            }
        }

        foreach (var grid in m_Grids)
        {
            var coord = grid.m_PanelCoord;
            bool isEdge = false;
            for (int i = 0; i < 4; i++)
            {
                var neighborCorrd = coord + offsets[i];
                if(!hashFind.Contains(neighborCorrd))
                {
                    isEdge = true;
                    break;
                }
            }
            grid.m_IsEdge = isEdge;
        }

    }

    [HideInInspector]
    public GameObject m_Debugger;

    public void UpdateDebug(Mesh mesh,Material material,Transform parent)
    {
        if(m_Debugger == null)
        {
            m_Debugger = new GameObject("ElementalPanel");
            m_Debugger.transform.SetParent(parent);
            m_Debugger.transform.localPosition = Vector3.zero;
        }

        for (int i = m_Debugger.transform.childCount - 1; i > -1; i--)
        {
            var child = m_Debugger.transform.GetChild(i);
            GameObject.DestroyImmediate(child.gameObject);
        }

        foreach (var grid in m_Grids)
        {
            grid.UpdateDebug(mesh, material, m_Debugger.transform, m_GridSize);
        }
    }

}
