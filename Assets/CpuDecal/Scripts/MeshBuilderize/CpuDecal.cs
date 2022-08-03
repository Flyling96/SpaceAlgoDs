using Hierarchy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Geometry;

namespace MeshBuilderize
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(BuilderMesh))]
    public partial class CpuDecal : MonoBehaviour
    {
        [SerializeField]
        private BuilderMeshManager manager = null;

        public MeshFilter m_DecalMeshFilter = null;

        public MeshRenderer m_DecalMeshRender = null;

        public bool CollideBVH(List<IBVHContent> collideContents = null)
        {
            if(manager == null)
            {
                return false;
            }

            var builderMesh = transform.GetComponent<BuilderMesh>();
            if(collideContents == null)
            {
                collideContents = new List<IBVHContent>();
            }
            bool isCollide = manager.CollideBVHTree(builderMesh.AABB, collideContents);
            if(isCollide)
            {
                foreach (var content in collideContents)
                {
                    Debug.LogError($"Collide {content.Mono.gameObject.name}");
                }
            }

            return isCollide;
        }

        public void CaculateDecal()
        {
            List<IBVHContent> collideContents = new List<IBVHContent>();
            var isCollide = CollideBVH(collideContents);
            if(!isCollide)
            {
                return;
            }

            List<Polygon> allPolygons = new List<Polygon>();
            for (int i = 0; i < collideContents.Count; i++)
            {
                if(collideContents[i] is BuilderMesh mesh)
                {
                    var polygons = CaculateDecal(mesh);
                    if(polygons != null && polygons.Count > 0)
                    {
                        allPolygons.AddRange(polygons);
                    }
                }
            }

            if(allPolygons.Count > 0)
            {
                RefreshDecalMesh(allPolygons);
            }
        }

        public List<Polygon> CaculateDecal(BuilderMesh other)
        {
            var self = transform.GetComponent<BuilderMesh>();
            if (self.m_Model == null || !other.IsStatic)
            {
                return null;
            }

            self.m_Model.UpdateTranform(transform);
            BSP selfBSP = new BSP(self.m_Model);
            BSP otherBSP = other.m_BSPTree.Clone();

            selfBSP.Root.Invert();
            otherBSP.Root.ClipTo(selfBSP.Root);

            return otherBSP.Root.AllPolygons();
        }

        private void RefreshDecalMesh(List<Polygon> polygons)
        {
            List<int> indices = new List<int>();
            List<Vector3> vertices = new List<Vector3>();

            int index = 0;
            for (int i = polygons.Count - 1; i > -1; i--)
            {
                var polygon = polygons[i];
                var normal = polygon.m_Plane.m_Normal.normalized;
                int offset = index;
                for (int j = 0; j < polygon.m_Vertices.Count; j++)
                {
                    var vertex = polygon.m_Vertices[j];
                    vertex.Position += normal * 0.01f;
                    vertex.Position = transform.InverseTransformPoint(vertex.Position);
                    if (j > 1)
                    {
                        indices.Add(offset);
                        indices.Add(offset + j - 1);
                        indices.Add(offset + j);
                    }
                    vertices.Add(vertex.Position);
                    index++;
                }
            }

            if (polygons.Count > 0 && m_DecalMeshFilter != null)
            {
                Mesh mesh = new Mesh();
                mesh.vertices = vertices.ToArray();
                mesh.SetIndices(indices, MeshTopology.Triangles, 0);
                m_DecalMeshFilter.mesh = mesh;
            }
        }

    }

#if UNITY_EDITOR
    public partial class CpuDecal
    {
        private void Awake()
        {
            if (Application.isPlaying)
            {
                return;
            }

            OnTransformParentChanged();

            if(m_DecalMeshFilter == null)
            {
                GameObject decal = new GameObject("Decal");
                decal.transform.SetParent(transform);
                decal.transform.localPosition = Vector3.zero;
                decal.transform.localRotation = Quaternion.identity;
                decal.transform.localScale = Vector3.one;
                m_DecalMeshFilter = decal.AddComponent<MeshFilter>();
                m_DecalMeshRender = decal.AddComponent<MeshRenderer>();
            }
        }

        private void OnTransformParentChanged()
        {
            if(Application.isPlaying)
            {
                return;
            }

            manager = null;
            var parent = transform.parent;
            if(parent.name == BuilderMeshManager.DecalRootName)
            {
                if(parent.parent != null)
                {
                    manager = parent.parent.GetComponent<BuilderMeshManager>();
                }
            }

        }
    }
#endif
}
