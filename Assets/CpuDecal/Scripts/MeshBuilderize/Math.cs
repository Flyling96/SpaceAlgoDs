using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshBuilderize
{
    public static class Math 
    {
        const float k_FltCompareEpsilon = 0.0001f;

        internal static bool Approx3(this Vector3 a, Vector3 b, float delta = k_FltCompareEpsilon)
        {
            return
                Mathf.Abs(a.x - b.x) < delta &&
                Mathf.Abs(a.y - b.y) < delta &&
                Mathf.Abs(a.z - b.z) < delta;
        }
    }
}
