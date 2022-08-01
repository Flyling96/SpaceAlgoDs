using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Geometry;
using CustomHierarchy;

namespace MeshBuilderize
{
    [RequireComponent(typeof(MeshFilter))]
    public class BuilderMesh : MonoBehaviour,IBVHContent
    {
        public Model m_Model = null;

        public AABB AABB => m_Model != null ? m_Model.m_AABB : default;

        public MonoBehaviour Mono => this;

        public void Builderize()
        {
            var mesh = transform.GetComponent<MeshFilter>().sharedMesh;
            if(mesh == null)
            {
                return;
            }

            m_Model = new Model(mesh, transform);
        }

    }
}
