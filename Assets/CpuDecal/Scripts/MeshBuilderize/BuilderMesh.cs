using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Geometry;

namespace MeshBuilderize
{
    [RequireComponent(typeof(MeshFilter))]
    public class BuilderMesh : MonoBehaviour
    {
        public Model m_Model = null;

        public void Builderize()
        {
            var mesh = transform.GetComponent<MeshFilter>().mesh;
            if(mesh == null)
            {
                return;
            }

            m_Model = new Model(mesh, transform);
        }
    }
}
