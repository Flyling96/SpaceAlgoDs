using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Geometry
{
    public class Plane
    {
        public Vector3 m_Normal;
        public float m_W;

        public Plane()
        {
            m_Normal = Vector3.zero;
            m_W = 0f;
        }

        public Plane(Vector3 a, Vector3 b, Vector3 c)
        {
            m_Normal = Vector3.Cross(b - a, c - a);//.normalized;
            m_W = Vector3.Dot(m_Normal, a);
        }

        public void Flip()
        {
            m_Normal *= -1f;
            m_W *= -1f;
        }
    }
}
