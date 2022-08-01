using Hierarchy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshBuilderize
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(BuilderMesh))]
    public partial class CpuDecal : MonoBehaviour
    {
        [SerializeField]
        private BuilderMeshManager manager = null;

        public bool CollideBVH()
        {
            if(manager == null)
            {
                return false;
            }

            var builderMesh = transform.GetComponent<BuilderMesh>();
            List<IBVHContent> collideContents = new List<IBVHContent>();
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
