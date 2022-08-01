using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Geometry;
using System;

namespace Hierarchy
{
    public interface IBVHContent
    {
        AABB AABB { get; }

        MonoBehaviour Mono { get; }
    }
   
    [Serializable]
    public partial class BVH
    {
        [SerializeReference]
        BVHNode m_Root;

        public void BuildTree(List<IBVHContent> contents, int nodeContentCount = 1)
        {
            m_Root = Build(contents, 0, nodeContentCount);
        }

        private BVHNode Build(List<IBVHContent> contents, int depth, int nodeContentCount = 1)
        {
            if(contents.Count <= nodeContentCount)
            {
                return new BVHNode(contents, depth);
            }

            var children = Divide(contents);
            var left = Build(children.Item1, depth + 1, nodeContentCount);
            var right = Build(children.Item2, depth + 1,nodeContentCount);

            return new BVHNode(left, right, depth);
        }

        public float GetSurfaceArea(Vector3 min,Vector3 max)
        {
            Vector3 diff = max - min;
            return (diff.x * diff.y + diff.x * diff.z + diff.y * diff.y) * 2;
        }

        //SAH
        private (List<IBVHContent>,List<IBVHContent>) Divide(List<IBVHContent> contents)
        {
            int minAxis = 0;
            int minMid = 0;
            float minCost = float.MaxValue;

            float[] reverseAreas = new float[contents.Count];
            //x,y,z
            for (int axis = 0; axis < 3; axis++)
            {
                if (axis == 0) contents.Sort(SortXAxis);
                else if (axis == 1) contents.Sort(SortYAxis);
                else if (axis == 2) contents.Sort(SortZAxis);

                for (int i = 0; i < reverseAreas.Length; i++)
                {
                    reverseAreas[i] = 0;
                }

                Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
                Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
                for (int i = contents.Count - 1; i > -1; i--)
                {
                    max = Vector3.Max(max, contents[i].AABB.m_Max);
                    min = Vector3.Min(min, contents[i].AABB.m_Min);
                    reverseAreas[i] = GetSurfaceArea(min, max);
                }

                max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
                min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

                for (int i = 0; i < contents.Count - 1; i++)
                {
                    max = Vector3.Max(max, contents[i].AABB.m_Max);
                    min = Vector3.Min(min, contents[i].AABB.m_Min);
                    float forwardArea = GetSurfaceArea(min, max);
                    //cost = (area0 * n0 + area1 * n1)/sumArea
                    float cost = forwardArea * (i + 1) + reverseAreas[i + 1] * (contents.Count - i);
                    if(cost < minCost)
                    {
                        minCost = cost;
                        minMid = i + 1;
                        minAxis = axis;
                    }
                }
            }

            if (minAxis == 0) contents.Sort(SortXAxis);
            else if (minAxis == 1) contents.Sort(SortYAxis);
            else if (minAxis == 2) contents.Sort(SortZAxis);

            var left = contents.GetRange(0, minMid);
            var right = contents.GetRange(minMid, contents.Count - minMid);

            return (left, right);

        }

        private int SortXAxis(IBVHContent a, IBVHContent b)
        {
            if(a.AABB.m_Max.x < b.AABB.m_Max.x)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        private int SortYAxis(IBVHContent a, IBVHContent b)
        {
            if (a.AABB.m_Max.y < b.AABB.m_Max.y)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        private int SortZAxis(IBVHContent a, IBVHContent b)
        {
            if (a.AABB.m_Max.z < b.AABB.m_Max.z)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        #region Collide
        public bool CollideAABB(AABB aabb, List<IBVHContent> contents)
        {
            if(m_Root == null)
            {
                return false;
            }

            return m_Root.Collide(aabb, contents);
        }
        #endregion
    }

#if UNITY_EDITOR
    public partial class BVH
    {
        public void DrawGizmos(int depth = -1)
        {
            if(m_Root == null)
            {
                return;
            }

            m_Root.DrawCustomGizmos(depth);
        }
    }
#endif
}
