using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Geometry
{
    public class Polygon
    {
        public List<Vertex> m_Vertices;
        public Plane m_Plane;

        public Polygon(List<Vertex> list)
        {
            m_Vertices = list;
            m_Plane = new Plane(list[0].Position, list[1].Position, list[2].Position);
        }

        public void Flip()
        {
            m_Vertices.Reverse();
            //for (int i = 0; i < m_Vertices.Count; i++)
            //    m_Vertices[i].Flip();
            m_Plane.Flip();
        }

        public override string ToString()
        {
            return "normal: " + m_Plane.m_Normal;
        }
    }
}
