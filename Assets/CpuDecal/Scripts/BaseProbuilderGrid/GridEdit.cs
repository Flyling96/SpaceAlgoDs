#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public enum ElementalType
{
    Grass,
    Water,
    Fire,
    Dust,
}

[ExecuteInEditMode]
[RequireComponent(typeof(ProBuilderMesh))]
public class GridEdit : MonoBehaviour
{
    private Vector3 m_LastPosition;

    private ProBuilderMesh m_ModeMesh;

    private const float RefreshDistance = 0.02f;

    [SerializeField]
    private ProBuilderMesh m_PolygonMesh;

    [SerializeField]
    private MeshFilter m_DebugMesh;

    private void Awake()
    {
        m_ModeMesh = transform.GetComponent<ProBuilderMesh>();
        RefreshPolygon();
    }

    private void Update()
    {
        if(Vector3.Distance(m_LastPosition,transform.position) > RefreshDistance)
        {
            m_LastPosition = transform.position;
            RefreshPolygon();
        }
    }

    private void RefreshPolygon()
    {
        if(m_PolygonMesh == null)
        {
            m_PolygonMesh = ProBuilderMesh.Create();
            m_PolygonMesh.gameObject.name = "Debug Polygon";
            m_PolygonMesh.transform.parent = transform;
            //m_PolygonMesh.transform.localPosition = Vector3.zero;
            //m_PolygonMesh.transform.localRotation = Quaternion.identity;
        }

        if (m_DebugMesh == null)
        {
            var go = new GameObject("Debug Mesh");
            go.transform.parent = m_PolygonMesh.transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.AddComponent<MeshRenderer>();
            m_DebugMesh = go.AddComponent<MeshFilter>();
        }

        var mesh = m_ModeMesh.RefreshUpIntersect(GridEditManager.Instance.m_SceneMesh);
        if(mesh != null)
        {
            //TODO:减面算法得对顶点三角面加权，不然会丢失形状
            //mesh = Simplify(mesh,0.5f);

            m_PolygonMesh.Clear();
            m_PolygonMesh.transform.position = Vector3.zero;
            m_PolygonMesh.transform.rotation = Quaternion.identity;

            m_PolygonMesh.GetComponent<MeshFilter>().sharedMesh = mesh;
            m_PolygonMesh.GetComponent<MeshRenderer>().sharedMaterial = GridEditManager.Instance.Material;
            MeshImporter importer = new MeshImporter(m_PolygonMesh.gameObject);
            importer.Import(new MeshImportSettings() { quads = true, smoothing = true, smoothingAngle = 1f });
            m_PolygonMesh.Rebuild();
            m_PolygonMesh.CenterPivot(null);

            //var points = Simplify(m_PolygonMesh.mesh);
            //for (int i = 0; i < m_PolygonMesh.transform.childCount; i++)
            //{
            //    var child = m_PolygonMesh.transform.GetChild(i);
            //    child.GetComponent<MeshRenderer>().enabled = false;
            //}

            //for (int i = 0; i < points.Count; i++)
            //{
            //    string debugName = $"Debug_{i}";
            //    var trans = transform.Find(debugName);
            //    if (trans == null)
            //    {
            //        trans = new GameObject(debugName).transform;
            //        trans.parent = m_PolygonMesh.transform;
            //        trans.gameObject.AddComponent<MeshFilter>().sharedMesh = debugMesh;
            //        trans.gameObject.AddComponent<MeshRenderer>().sharedMaterial = debugMaterial;
            //    }

            //    trans.localScale = Vector3.one * 0.3f;
            //    trans.localPosition = points[i];
            //    trans.GetComponent<MeshRenderer>().enabled = true;
            //}
        }
        else
        {
            if(m_PolygonMesh.mesh != null)
            {
                m_PolygonMesh.Clear();
                m_PolygonMesh.Rebuild();
                m_PolygonMesh.CenterPivot(null);
            }
        }
    }


    //private Mesh Simplify(Mesh sourceMesh,float quality)
    //{
    //    if (sourceMesh == null)
    //        return null;

    //    var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();
    //    meshSimplifier.Vertices = sourceMesh.vertices;

    //    for (int i = 0; i < sourceMesh.subMeshCount; i++)
    //    {
    //        meshSimplifier.AddSubMeshTriangles(sourceMesh.GetTriangles(i));
    //    }


    //    meshSimplifier.SimplifyMesh(quality);

    //    var newMesh = new Mesh();
    //    newMesh.subMeshCount = meshSimplifier.SubMeshCount;
    //    newMesh.vertices = meshSimplifier.Vertices;

    //    for (int i = 0; i < meshSimplifier.SubMeshCount; i++)
    //    {
    //        newMesh.SetTriangles(meshSimplifier.GetSubMeshTriangles(i), 0);
    //    }

    //    return newMesh;
    //}

    private List<Vector3> Simplify(Mesh sourceMesh)
    {
        var vertices = new List<Vector3>();
        var points = new List<Vector2>();
        for (int i = 0; i < sourceMesh.vertices.Length; i++)
        {
            var pos = sourceMesh.vertices[i];
            var index = IndexOfVertices(pos, vertices);
            if(index == -1)
            {
                vertices.Add(pos);
                points.Add(new Vector2(pos.x, pos.z));
            }
        }

        var indices = new List<int>();
        var sourceIndices = sourceMesh.GetIndices(0);
        for (int i = 0; i < sourceIndices.Length; i++)
        {
            var pos = sourceMesh.vertices[sourceIndices[i]];
            var index = IndexOfVertices(pos, vertices);
            if (index != -1)
            {
                indices.Add(index);
            }
            else
            {
                Debug.LogError("Simplify Mesh Error");
            }
        }
        var edges = new List<Vector2Int>();
        for (int i = 0; i < indices.Count / 3; i++)
        {
            var index0 = indices[i * 3];
            var index1 = indices[i * 3 + 1];
            var index2 = indices[i * 3 + 2];
            edges.Add(new Vector2Int(index0, index1));
            edges.Add(new Vector2Int(index1, index2));
            edges.Add(new Vector2Int(index2, index0));
        }
        points = GeometryHelper.RollEdge(points, edges);
        //points = GeometryHelper.BallConcave(points);
        var y = vertices[0].y;
        vertices.Clear();
        indices.Clear();
        for (int i = 0; i < points.Count; i++)
        {
            var point = points[i];
            vertices.Add(new Vector3(point.x, y, point.y));
            if(i > 1)
            {
                indices.Add(0);
                indices.Add(i);
                indices.Add(i - 1);
            }
        }

        return vertices;
    }

    private int IndexOfVertices(Vector3 position, List<Vector3> vertices)
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            var vertex = vertices[i];
            if (Vector3.Distance(vertex, position) <= 0.01f)
            {
                return i;
            }
        }

        return -1;
    }

    #region Serialize
    public void Serialize(BinaryWriter writer)
    {
        if(m_PolygonMesh == null || m_PolygonMesh.faces.Count < 1)
        {
            writer.Write(false);
            return;
        }

        writer.Write(m_PolygonMesh.positions.Count);
        for (int i = 0; i < m_PolygonMesh.positions.Count; i++)
        {
            writer.Write(m_PolygonMesh.positions[i].x);
            writer.Write(m_PolygonMesh.positions[i].z);
        }

        writer.Write(m_PolygonMesh.faces.Count);

        for (int i = 0; i < m_PolygonMesh.faces.Count; i++)
        {
            var face = m_PolygonMesh.faces[i];
            //writer.Write(face.indexes);
        }
    }
    #endregion

    #region Grid

    public Mesh m_GridMesh = null;
    public Material m_GridMaterial = null;
    [SerializeField]
    private GridPanel m_Panel = new GridPanel();

    public void GerneateGrid()
    {
        if(m_PolygonMesh.mesh == null)
        {
            return;
        }

        m_Panel.GenerateGrid(m_PolygonMesh.mesh, m_PolygonMesh.transform.position);
        m_Panel.UpdateDebug(m_GridMesh, m_GridMaterial, m_PolygonMesh.transform);

    }

    #endregion
}
#endif