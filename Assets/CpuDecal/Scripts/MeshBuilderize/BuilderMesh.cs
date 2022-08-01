using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Geometry;
using Hierarchy;

namespace MeshBuilderize
{
    [RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
    public partial class BuilderMesh : MonoBehaviour,IBVHContent
    {
        public Model m_Model = null;

        public AABB AABB
        {
            get
            {
                if(m_Model == null)
                {
                    return AABB.Identity();
                }

                if(m_Model.m_IsStatic)
                {
                    return m_Model.m_AABB;
                }
                else
                {
                    return transform.TransformAABB(m_Model.m_AABB);
                }
            }
        }

        public MonoBehaviour Mono => this;

        public void Builderize(bool isStatic = false)
        {
            var mesh = transform.GetComponent<MeshFilter>().sharedMesh;
            if(mesh == null)
            {
                return;
            }

            m_Model = new Model(mesh, transform, isStatic);
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
