using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomHierarchy;
using Geometry;
using UnityEditor;

namespace MeshBuilderize
{
    public partial class BuilderMeshManager : MonoBehaviour
    {
        public List<BuilderMesh> m_BuilderMeshList = new List<BuilderMesh>();

        [SerializeField]
        private BVH m_BVHTree;

        public void BuildBVHTree()
        {
            m_BVHTree = new BVH();

            var bvhContentList = new List<IBVHContent>();
            for (int i = 0; i < m_BuilderMeshList.Count; i++)
            {
                var builderMesh = m_BuilderMeshList[i];
                if(builderMesh.m_Model == null)
                {
                    continue;
                }

                bvhContentList.Add(builderMesh);
            }

            m_BVHTree.BuildTree(bvhContentList);
        }

    }

#if UNITY_EDITOR
    [ExecuteInEditMode]
    public partial class BuilderMeshManager
    {
        public int m_DebugDepth = -1;

        public GameObject m_ModelRoot;
        public GameObject m_DecalRoot;

        private void Awake()
        {
            if(m_ModelRoot == null)
            {
                m_ModelRoot = new GameObject("Models");
                m_ModelRoot.transform.SetParent(transform);
                m_ModelRoot.transform.localPosition = Vector3.zero;
                m_ModelRoot.transform.localRotation = Quaternion.identity;
                m_ModelRoot.transform.localScale = Vector3.one;
            }

            if(m_DecalRoot == null)
            {
                m_DecalRoot = new GameObject("Decals");
                m_DecalRoot.transform.SetParent(transform);
                m_DecalRoot.transform.localPosition = Vector3.zero;
                m_DecalRoot.transform.localRotation = Quaternion.identity;
                m_DecalRoot.transform.localScale = Vector3.one;
            }
        }

        public void Builderize()
        {
            m_BuilderMeshList.Clear();
            var meshFilters = m_ModelRoot.transform.GetComponentsInChildren<MeshFilter>();
            if (meshFilters == null)
            {
                return;
            }

            foreach (var meshFilter in meshFilters)
            {
                var mesh = meshFilter.sharedMesh;
                if (mesh == null)
                {
                    return;
                }

                var builderMesh = meshFilter.GetComponent<BuilderMesh>();
                if (builderMesh == null)
                {
                    builderMesh = Undo.AddComponent<BuilderMesh>(meshFilter.gameObject);
                }

                builderMesh.Builderize();
                m_BuilderMeshList.Add(builderMesh);
            }

        }

        private void OnDrawGizmosSelected()
        {
            if(m_BVHTree == null)
            {
                return;
            }

            m_BVHTree.DrawGizmos(m_DebugDepth);
        }
    }
#endif
}
