using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

namespace MeshBuilderize
{
    [Serializable]
    public class Face
    {
        [SerializeField]
        int[] m_Indexes;

        public Face(IEnumerable<int> indices)
        {
            SetIndexes(indices);
        }

        public void SetIndexes(IEnumerable<int> indices)
        {
            if (indices == null)
            {
                Debug.LogError("Indices is null.");
                return;
            }

            var array = indices.ToArray();
            int len = array.Length;
            if (len % 3 != 0)
            {
                Debug.LogError("Face indexes must be a multiple of 3.");
                return;
            }
            m_Indexes = array;
        }
    }
}
