using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Geometry;
using Hierarchy;

namespace MeshBuilderize
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
    public partial class BuilderMesh : MonoBehaviour,IBVHContent
    {
        public Model m_Model = null;

        public BSP m_BSPTree = null;

        public bool IsStatic => m_Model != null ? m_Model.m_IsStatic : false;

        public AABB AABB
        {
            get
            {
                if(m_Model == null)
                {
                    return AABB.Identity();
                }

                return m_Model.m_AABB;
            }
        }

        public MonoBehaviour Mono => this;

        public void Builderize(bool isStatic = false,Mesh mesh = null)
        {
            if (mesh == null)
            {
                mesh = transform.GetComponent<MeshFilter>().sharedMesh;
                if (mesh == null)
                {
                    return;
                }
            }

            m_Model = new Model(mesh, transform, isStatic);
            if (isStatic)
            {
                m_BSPTree = new BSP(m_Model);
            }
        }


        private Vector3 m_LastPosition;
        private Quaternion m_LastRotation;
        private Vector3 m_LastScale;

        private void Update()
        {
            TickTransformChange();
        }

        private void TickTransformChange()
        {
            if (m_Model.m_IsStatic)
            {
                return;
            }

            bool isChange = false;
            if(!m_LastPosition.SimpleEqule(transform.position))
            {
                m_LastPosition = transform.position;
                isChange = true;
            }
            else if (!m_LastRotation.SimpleEqule(transform.rotation))
            {
                m_LastRotation = transform.rotation;
                isChange = true;
            }
            else if (!m_LastScale.SimpleEqule(transform.localScale))
            {
                m_LastScale = transform.localScale;
                isChange = true;
            }

            if(isChange)
            {
                m_Model.UpdateTranform(transform);
            }

        }

    }

#if UNITY_EDITOR
    public partial class BuilderMesh
    {
        private void OnDrawGizmosSelected()
        {
            if(m_Model != null)
            {
                var aabb = AABB;
                Gizmos.color = Color.cyan;
                var center = (aabb.m_Min + aabb.m_Max) / 2;
                var size = (aabb.m_Max - aabb.m_Min);
                Gizmos.DrawWireCube(center, size);
            }
        }
    }
#endif
}
