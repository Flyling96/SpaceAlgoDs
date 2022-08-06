using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hierarchy;
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

        public bool CollideBVHTree(AABB aabb, List<IBVHContent> contents)
        {
            if(m_BVHTree == null)
            {
                Debug.LogError("BVHTree Don't Build");
                return false;
            }

            return m_BVHTree.CollideAABB(aabb, contents);

        }

    }

#if UNITY_EDITOR
    [ExecuteInEditMode]
    public partial class BuilderMeshManager
    {
        public const string ModelRootName = "StaticModels";
        public const string DecalRootName = "Decals";

        public int m_DebugDepth = -1;

        public GameObject m_Scene;
        public GameObject m_ModelRoot;
        public GameObject m_DecalRoot;

        private void Start()
        {
            if(m_ModelRoot == null)
            {
                m_ModelRoot = new GameObject(ModelRootName);
                m_ModelRoot.transform.SetParent(transform);
                m_ModelRoot.transform.localPosition = Vector3.zero;
                m_ModelRoot.transform.localRotation = Quaternion.identity;
                m_ModelRoot.transform.localScale = Vector3.one;
            }

            if(m_DecalRoot == null)
            {
                m_DecalRoot = new GameObject(DecalRootName);
                m_DecalRoot.transform.SetParent(transform);
                m_DecalRoot.transform.localPosition = Vector3.zero;
                m_DecalRoot.transform.localRotation = Quaternion.identity;
                m_DecalRoot.transform.localScale = Vector3.one;
            }
        }

        public void StaticModelBuilderize()
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

                builderMesh.Builderize(true);
                m_BuilderMeshList.Add(builderMesh);
            }

        }

        private class SceneTreeNode
        {
            public Transform m_Content;

            public List<SceneTreeNode> m_Children = new List<SceneTreeNode>();


            public bool Build(Transform transform)
            {
                m_Content = transform;

                if (transform.childCount == 0)
                {
                    var filter = transform.GetComponent<MeshFilter>();
                    if(filter != null && filter.sharedMesh != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                for (int i = 0; i < transform.childCount; i++)
                {
                    var child = transform.GetChild(i);
                    SceneTreeNode childNode = new SceneTreeNode();
                    if(childNode.Build(child))
                    {
                        m_Children.Add(childNode);
                    }
                }

                if(m_Children.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }

            public void BuildMesh(Transform parent,List<BuilderMesh> list)
            {
                var clone = new GameObject(m_Content.name);
                clone.transform.SetParent(parent);
                clone.transform.localPosition = m_Content.localPosition;
                clone.transform.localRotation = m_Content.localRotation;
                clone.transform.localScale = m_Content.localScale;
                var filter = m_Content.GetComponent<MeshFilter>();
                if(filter != null && filter.sharedMesh != null)
                {
                    var builderMesh = clone.AddComponent<BuilderMesh>();
                    builderMesh.Builderize(true,filter.sharedMesh);
                    list.Add(builderMesh);
                }

                for (int i = 0; i < m_Children.Count; i++)
                {
                    var child = m_Children[i];
                    child.BuildMesh(clone.transform, list);
                }
            }
        }

        public void StaticModelCloneBuilderize()
        {
            if(m_Scene == null)
            {
                return;
            }

            SceneTreeNode root = new SceneTreeNode();
            root.Build(m_Scene.transform);
            m_BuilderMeshList.Clear();
            root.BuildMesh(m_ModelRoot.transform, m_BuilderMeshList);
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
