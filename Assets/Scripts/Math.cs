﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct AABB
{
    public Vector3 m_Min;
    public Vector3 m_Max;

    public float GetSurfaceArea()
    {
        Vector3 diff = m_Max - m_Min;
        return (diff.x * diff.y + diff.x * diff.z + diff.y * diff.y) * 2;
    }

    public bool IsCollide(AABB other)
    {
        if (other.m_Max.x < m_Min.x || other.m_Max.y < m_Min.y || other.m_Max.z < m_Min.z ||
           other.m_Min.x > m_Max.x || other.m_Min.y > m_Max.y || other.m_Min.z > m_Max.z)
        {
            return false;
        }

        return true;
    }
}

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
