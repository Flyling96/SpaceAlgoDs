using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Geometry;
using System;

namespace Hierarchy
{
    [Serializable]
    public partial class BVHNode : ISerializationCallbackReceiver
    {
        public AABB m_AABB;

        public int m_Depth;

        public List<IBVHContent> m_Contents = null;

        [SerializeReference]
        public BVHNode m_Left = null;

        [SerializeReference]
        public BVHNode m_Right = null;

        public BVHNode(List<IBVHContent> contents,int depth)
        {
            m_Contents = contents;
            m_Depth = depth;
            m_AABB.m_Min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            m_AABB.m_Max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            for (int i = 0; i < m_Contents.Count; i++)
            {
                m_AABB.m_Min = Vector3.Min(m_AABB.m_Min, m_Contents[i].AABB.m_Min);
                m_AABB.m_Max = Vector3.Max(m_AABB.m_Max, m_Contents[i].AABB.m_Max);
            }
        }

        public BVHNode(BVHNode left,BVHNode right,int depth)
        {
            m_Left = left;
            m_Right = right;
            m_Depth = depth;
            m_AABB.m_Min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            m_AABB.m_Max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            m_AABB.m_Min = Vector3.Min(left.m_AABB.m_Min, right.m_AABB.m_Min);
            m_AABB.m_Max = Vector3.Max(left.m_AABB.m_Max, right.m_AABB.m_Max);
        }

        #region Collide
        public bool Collide(AABB other,List<IBVHContent> contents)
        {
            if(!m_AABB.IsCollide(other))
            {
                return false;
            }

            if(m_Contents != null)
            {
                contents.AddRange(m_Contents);
                return true;
            }
            else
            {
                bool res = false;
                if (m_Left != null)
                {
                    res |= m_Left.Collide(other, contents);
                }
                if(m_Right != null)
                {
                    res |= m_Right.Collide(other, contents);
                }

                return res;
            }
        }

        [SerializeField]
        private List<MonoBehaviour> m_SerializeContent = new List<MonoBehaviour>();

        public void OnAfterDeserialize()
        {
            if(m_SerializeContent.Count < 1)
            {
                return;
            }

            m_Contents = new List<IBVHContent>();
            for (int i = 0; i < m_SerializeContent.Count; i++)
            {
                var mono = m_SerializeContent[i];
                if(mono == null)
                {
                    continue;
                }
                var content = mono as IBVHContent;
                if(content == null)
                {
                    continue;
                }
                m_Contents.Add(content);
            }
        }

        public void OnBeforeSerialize()
        {
            if(m_Contents == null)
            {
                return;
            }
            m_SerializeContent.Clear();
            for (int i = 0; i < m_Contents.Count; i++)
            {
                var content = m_Contents[i];
                if (content != null)
                {
                    m_SerializeContent.Add(content.Mono);
                }
            }
        }
        #endregion
    }

#if UNITY_EDITOR
    public partial class BVHNode
    {
        public void DrawCustomGizmos(int depth = -1)
        {
            if(depth != -1 && m_Depth != depth)
            {
                m_Left?.DrawCustomGizmos(depth);
                m_Right?.DrawCustomGizmos(depth);
                return;
            }

            Gizmos.color = GetGizmosColor();
            var center = (m_AABB.m_Min + m_AABB.m_Max) / 2;
            var size = (m_AABB.m_Max - m_AABB.m_Min);
            Gizmos.DrawWireCube(center, size);

            if (m_Contents != null)
            {
                Gizmos.color = Color.cyan;
                for (int i = 0; i < m_Contents.Count; i++)
                {
                    var aabb = m_Contents[i].AABB;
                    center = (aabb.m_Min + aabb.m_Max) / 2;
                    size = (aabb.m_Max - aabb.m_Min);
                    Gizmos.DrawWireCube(center, size);
                }
            }

            m_Left?.DrawCustomGizmos(depth);
            m_Right?.DrawCustomGizmos(depth);
        }

        public Color GetGizmosColor()
        {
            switch(m_Depth)
            {
                case 0:return Color.white;
                case 1:return Color.green;
                case 2:return Color.blue;
                case 3:return Color.magenta;
                case 4:return Color.red;
                case 5:return Color.yellow;
                default:return Color.black;
            }
        }
    }
#endif
}
