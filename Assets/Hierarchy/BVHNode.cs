using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Geometry;

namespace Hierarchy
{
    public partial class BVHNode
    {
        public AABB m_AABB;

        public List<IBVHContent> m_Contents;

        public BVHNode m_Left;

        public BVHNode m_Right;

        public BVHNode(List<IBVHContent> contents)
        {
            m_Contents = contents;
            for (int i = 0; i < m_Contents.Count; i++)
            {
                m_AABB.m_Min = Vector3.Min(m_AABB.m_Min, m_Contents[i].AABB.m_Min);
                m_AABB.m_Max = Vector3.Max(m_AABB.m_Max, m_Contents[i].AABB.m_Max);
            }
        }

        public BVHNode(BVHNode left,BVHNode right)
        {
            m_Left = left;
            m_Right = right;
            m_AABB.m_Min = Vector3.Min(left.m_AABB.m_Min, right.m_AABB.m_Min);
            m_AABB.m_Max = Vector3.Max(left.m_AABB.m_Max, right.m_AABB.m_Max);
        }
    }

#if UNITY_EDITOR
    public partial class BVHNode
    {
        public void DrawGizmos()
        {
            Gizmos.color = Color.green;
            var center = (m_AABB.m_Min + m_AABB.m_Max) / 2;
            var size = (m_AABB.m_Max - m_AABB.m_Min);
            Gizmos.DrawWireCube(center, size);

            if (m_Contents != null)
            {
                Gizmos.color = Color.white;
                for (int i = 0; i < m_Contents.Count; i++)
                {
                    var aabb = m_Contents[i].AABB;
                    center = (aabb.m_Min + aabb.m_Max) / 2;
                    size = (aabb.m_Max - aabb.m_Min);
                    Gizmos.DrawWireCube(center, size);
                }
            }

            m_Left?.DrawGizmos();
            m_Right?.DrawGizmos();
        }
    }
#endif
}
