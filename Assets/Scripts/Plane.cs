using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Geometry
{
    [System.Flags]
    enum EPolygonType
    {
        Coplanar = 0,
        Front = 1,
        Back = 2,
        Spanning = 3         /// 3 is Front | Back - not a separate entry
    };

    [Serializable]
    public class Plane
    {
        internal const float k_Epsilon = 0.001f;

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

        public bool Valid()
        {
            return m_Normal.magnitude > 0f;
        }

        public void Flip()
        {
            m_Normal *= -1f;
            m_W *= -1f;
        }

        public void SplitPolygon(Polygon polygon, List<Polygon> coplanarFront, List<Polygon> coplanarBack, List<Polygon> front, List<Polygon> back)
        {
            EPolygonType polygonType = 0;
            List<EPolygonType> types = new List<EPolygonType>();

            for (int i = 0; i < polygon.m_Vertices.Count; i++)
            {
                float t = Vector3.Dot(this.m_Normal, polygon.m_Vertices[i].Position) - this.m_W;
                EPolygonType type = (t < -k_Epsilon) ? EPolygonType.Back : ((t > k_Epsilon) ? EPolygonType.Front : EPolygonType.Coplanar);
                polygonType |= type;
                types.Add(type);
            }

            switch (polygonType)
            {
                case EPolygonType.Coplanar:
                    {
                        if (Vector3.Dot(this.m_Normal, polygon.m_Plane.m_Normal) > 0)
                            coplanarFront.Add(polygon);
                        else
                            coplanarBack.Add(polygon);
                    }
                    break;

                case EPolygonType.Front:
                    {
                        front.Add(polygon);
                    }
                    break;

                case EPolygonType.Back:
                    {
                        back.Add(polygon);
                    }
                    break;

                case EPolygonType.Spanning:
                    {
                        List<Vertex> f = new List<Vertex>();
                        List<Vertex> b = new List<Vertex>();

                        for (int i = 0; i < polygon.m_Vertices.Count; i++)
                        {
                            int j = (i + 1) % polygon.m_Vertices.Count;

                            EPolygonType ti = types[i], tj = types[j];

                            Vertex vi = polygon.m_Vertices[i], vj = polygon.m_Vertices[j];

                            if (ti != EPolygonType.Back)
                            {
                                f.Add(vi);
                            }

                            if (ti != EPolygonType.Front)
                            {
                                b.Add(vi);
                            }

                            if ((ti | tj) == EPolygonType.Spanning)
                            {
                                float t = (this.m_W - Vector3.Dot(this.m_Normal, vi.Position)) / Vector3.Dot(this.m_Normal, vj.Position - vi.Position);

                                Vertex v = VertexUtility.Mix(vi, vj, t);

                                f.Add(v);
                                b.Add(v);
                            }
                        }

                        if (f.Count >= 3)
                        {
                            front.Add(new Polygon(f));
                        }

                        if (b.Count >= 3)
                        {
                            back.Add(new Polygon(b));
                        }
                    }
                    break;
            }   // End switch(polygonType)
        }
    }
}
