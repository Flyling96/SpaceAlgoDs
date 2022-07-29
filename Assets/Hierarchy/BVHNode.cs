using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Geometry;

namespace Hierarchy
{
    public class BVHNode
    {
        public AABB m_AABB;

        public List<IBVHContent> m_Contents;

        public BVHNode m_Left;

        public BVHNode m_Right;

        public BVHNode(List<IBVHContent> contents)
        {
            m_Contents = contents;
        }

        public BVHNode(BVHNode left,BVHNode right)
        {
            m_Left = left;
            m_Right = right;
            m_AABB.m_Min = Vector3.Min(left.m_AABB.m_Min, right.m_AABB.m_Min);
            m_AABB.m_Max = Vector3.Max(left.m_AABB.m_Max, right.m_AABB.m_Max);
        }
    }
}
