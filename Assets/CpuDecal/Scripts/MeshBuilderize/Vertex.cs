using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshBuilderize
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
    }
}