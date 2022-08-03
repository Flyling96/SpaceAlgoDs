using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Geometry;
using Plane = Geometry.Plane;
using System;

namespace Hierarchy
{
    [Serializable]
    public class BSPNode
    {
        public List<Polygon> m_Polygons;

        [SerializeReference]
        public BSPNode m_Front = null;

        [SerializeReference]
        public BSPNode m_Back = null;

        public Plane m_Plane;

        public BSPNode()
        {
            this.m_Front = null;
            this.m_Back = null;
        }

        public BSPNode(List<Polygon> list)
        {
            Build(list);
        }

        public BSPNode(List<Polygon> list, Plane m_Plane, BSPNode m_Front, BSPNode m_Back)
        {
            this.m_Polygons = list;
            this.m_Plane = m_Plane;
            this.m_Front = m_Front;
            this.m_Back = m_Back;
        }

        public BSPNode Clone()
        {
            BSPNode clone = new BSPNode(this.m_Polygons, this.m_Plane, this.m_Front?.Clone(), this.m_Back?.Clone());
            return clone;
        }

        public void Build(List<Polygon> list)
        {
            if (list.Count < 1)
                return;

            if (this.m_Plane == null || !this.m_Plane.Valid())
            {
                this.m_Plane = new Plane();
                this.m_Plane.m_Normal = list[0].m_Plane.m_Normal;
                this.m_Plane.m_W = list[0].m_Plane.m_W;
            }


            if (this.m_Polygons == null)
                this.m_Polygons = new List<Polygon>();

            List<Polygon> list_front = new List<Polygon>();
            List<Polygon> list_back = new List<Polygon>();

            for (int i = 0; i < list.Count; i++)
            {
                this.m_Plane.SplitPolygon(list[i], this.m_Polygons, this.m_Polygons, list_front, list_back);
            }

            if (list_front.Count > 0)
            {
                if (this.m_Front == null)
                    this.m_Front = new BSPNode();

                this.m_Front.Build(list_front);
            }

            if (list_back.Count > 0)
            {
                if (this.m_Back == null)
                    this.m_Back = new BSPNode();

                this.m_Back.Build(list_back);
            }
        }

        public void Invert()
        {
            for (int i = 0; i < this.m_Polygons.Count; i++)
                this.m_Polygons[i].Flip();

            this.m_Plane.Flip();

            if (this.m_Front != null)
            {
                this.m_Front.Invert();
            }

            if (this.m_Back != null)
            {
                this.m_Back.Invert();
            }

            BSPNode tmp = this.m_Front;
            this.m_Front = this.m_Back;
            this.m_Back = tmp;
        }

        public void ClipTo(BSPNode other)
        {
            this.m_Polygons = other.ClipPolygons(this.m_Polygons);

            if (this.m_Front != null)
            {
                this.m_Front.ClipTo(other);
            }

            if (this.m_Back != null)
            {
                this.m_Back.ClipTo(other);
            }
        }

        public List<Polygon> ClipPolygons(List<Polygon> list)
        {
            if (!this.m_Plane.Valid())
            {
                return list;
            }

            List<Polygon> list_front = new List<Polygon>();
            List<Polygon> list_back = new List<Polygon>();

            for (int i = 0; i < list.Count; i++)
            {
                this.m_Plane.SplitPolygon(list[i], list_front, list_back, list_front, list_back);
            }

            if (this.m_Front != null)
            {
                list_front = this.m_Front.ClipPolygons(list_front);
            }

            if (this.m_Back != null)
            {
                list_back = this.m_Back.ClipPolygons(list_back);
            }
            else
            {
                list_back.Clear();
            }

            // Position [First, Last]
            // list_front.insert(list_front.end(), list_back.begin(), list_back.end());
            list_front.AddRange(list_back);

            return list_front;
        }

        public List<Polygon> AllPolygons()
        {
            List<Polygon> list = this.m_Polygons;
            List<Polygon> list_front = new List<Polygon>(), list_back = new List<Polygon>();

            if (this.m_Front != null)
            {
                list_front = this.m_Front.AllPolygons();
            }

            if (this.m_Back != null)
            {
                list_back = this.m_Back.AllPolygons();
            }

            list.AddRange(list_front);
            list.AddRange(list_back);

            return list;
        }

    }
}
