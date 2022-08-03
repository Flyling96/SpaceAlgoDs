using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Geometry
{
    [Serializable]
    public sealed class Vertex : IEquatable<Vertex>
    {
        [SerializeField]
        Vector3 m_Position;

        public bool Equals(Vertex other)
        {
            if (ReferenceEquals(other, null))
                return false;

            return Math.Approx3(m_Position, other.m_Position);
        }

        public Vector3 Position
        {
            get 
            { 
                return m_Position; 
            }
            set
            {
                m_Position = value;
            }
        }

        public Vertex Clone()
        {
            Vertex res = new Vertex();
            res.Position = Position;
            return res;
        }
    }
}