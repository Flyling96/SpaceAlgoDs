using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Geometry;
using Plane = Geometry.Plane;

namespace Hierarchy
{
    public class BSPNode
    {
        public List<Polygon> m_Polygons;

        public BSPNode m_Front;
        public BSPNode m_Back;

        public Plane m_Plane;

        public BSPNode()
        {
            this.m_Front = null;
            this.m_Back = null;
        }

        public BSPNode(List<Polygon> list)
        {
            //Build(list);
        }

        public BSPNode(List<Polygon> list, Plane plane, BSPNode front, BSPNode back)
        {
            this.m_Polygons = list;
            this.m_Plane = plane;
            this.m_Front = front;
            this.m_Back = back;
        }

        public BSPNode Clone()
        {
            BSPNode clone = new BSPNode(this.m_Polygons, this.m_Plane, this.m_Front, this.m_Back);
            return clone;
        }
    }
}
