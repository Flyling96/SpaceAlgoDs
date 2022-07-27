using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GeometryHelper
{
    public static float Cross(this Vector2 v0, Vector2 v1)
    {
        return v0.x * v1.y - v1.x * v0.y;
    }


    #region ConvexHull
    public static Vector2[] GrahamScan(Vector2[] points)
    {
        Vector2 p0 = Vector2.zero; ;
        List<Vector2> pointList = new List<Vector2>();
        float minY = float.MaxValue;
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].y < minY)
            {
                p0 = points[i];
                minY = points[i].y;
            }
            pointList.Add(points[i]);
        }

        pointList.Sort(
           (Vector2 a, Vector2 b) =>
           {
               if (a == p0)
               {
                   return -1;
               }
               if (b == p0)
               {
                   return 1;
               }

               Vector2 dir0 = (a - p0).normalized;
               Vector2 dir1 = (b - p0).normalized;
               Vector2 xAxis = new Vector2(1, 0);

               float dot0 = Vector2.Dot(xAxis, dir0);
               float dot1 = Vector2.Dot(xAxis, dir1);

               return dot0 >= dot1 ? -1 : 1;
           }
        );


        for (int i = pointList.Count - 2; i > 0; i--)
        {
            var p1 = pointList[i + 1];
            var p2 = pointList[i];

            var dir1 = p1 - p0;
            var dir2 = p2 - p0;
            if (dir1.normalized == dir2.normalized)
            {
                if (dir1.magnitude > dir2.magnitude)
                {
                    pointList.RemoveAt(i);
                }
                else
                {
                    pointList.RemoveAt(i + 1);
                }
            }

        }
        if (pointList.Count < 3)
        {
            return new Vector2[0];
        }

        Stack<Vector2> pointStack = new Stack<Vector2>();
        Stack<Vector2> prePointStack = new Stack<Vector2>();
        pointStack.Push(p0);
        pointStack.Push(pointList[1]);
        prePointStack.Push(p0);

        for (int i = 2; i < pointList.Count; i++)
        {
            var prePoint = prePointStack.Peek();
            var p1 = pointStack.Peek();
            var p2 = pointList[i];
            var dir1 = p1 - prePoint;
            var dir2 = p2 - prePoint;
            float cross = Cross(dir1, dir2);

            if (cross > 0)
            {
                pointStack.Push(p2);
                prePointStack.Push(p1);
            }
            else
            {
                while (prePointStack.Count > 0)
                {
                    pointStack.Pop();
                    prePointStack.Pop();
                    p1 = pointStack.Peek();
                    prePoint = prePointStack.Peek();

                    dir1 = p1 - prePoint;
                    dir2 = p2 - prePoint;
                    cross = Cross(dir1, dir2);

                    if (cross > 0)
                    {
                        pointStack.Push(p2);
                        prePointStack.Push(p1);
                        break;
                    }
                }
            }
        }

        pointList.Clear();
        while (pointStack.Count > 0)
        {
            pointList.Add(pointStack.Pop());
        }

        return pointList.ToArray();

    }

    #endregion

    #region ConcaveHull
    public static List<Vector2> RollEdge(List<Vector2> points, List<Vector2Int> edges)
    {
        Vector2 p0 = Vector2.zero; ;
        List<Vector2> pointList = new List<Vector2>();
        float minY = float.MaxValue;
        for (int i = 0; i < points.Count; i++)
        {
            if (points[i].y < minY)
            {
                p0 = points[i];
                minY = points[i].y;
            }
            pointList.Add(points[i]);
        }

        pointList.Sort(
           (Vector2 a, Vector2 b) =>
           {
               if (a == p0)
               {
                   return -1;
               }
               if (b == p0)
               {
                   return 1;
               }

               Vector2 dir0 = (a - p0).normalized;
               Vector2 dir1 = (b - p0).normalized;
               Vector2 xAxis = new Vector2(1, 0);

               float dot0 = Vector2.Dot(xAxis, dir0);
               float dot1 = Vector2.Dot(xAxis, dir1);

               return dot0 >= dot1 ? -1 : 1;
           }
        );

        List<Vector2> res = new List<Vector2>();
        res.Add(p0);
        var start = p0;
        List<Vector2> searchPoints = new List<Vector2>();
        for (int i = 0; i < pointList.Count; i++)
        {
            searchPoints.Add(pointList[i]);
        }
        var vector = new Vector2(1, 0);

        while (true)
        {
            System.Comparison<Vector2> sort = (a, b) =>
            {
                var dir0 = (a - p0);
                var dir1 = (b - p0);

                var cross0 = Cross(vector, dir0);
                var cross1 = Cross(vector, dir1);
                if (cross0 < 0 && cross1 > 0)
                {
                    return -1;
                }
                else if (cross0 > 0 && cross1 < 0)
                {
                    return 1;
                }

                if (cross0 == 0)
                {
                    var dot0 = Vector2.Dot(vector, dir0);
                    if (dot0 < 0)
                    {
                        return 1;
                    }
                }

                if (cross1 == 0)
                {
                    var dot1 = Vector2.Dot(vector, dir1);
                    if (dot1 < 0)
                    {
                        return -1;
                    }
                }

                var cross = Cross(dir0, dir1);
                if(cross > 0)
                {
                    return -1;
                }
                else if(cross == 0)
                {
                    if(dir0.magnitude < dir1.magnitude)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    return 1;
                }

                /*var cross0 = Cross(vector, dir0);
                var cross1 = Cross(vector, dir1);

                if(cross0 < 0 && cross1 > 0)
                {
                    return -1;
                }
                else if(cross0 > 0 && cross1 < 0)
                {
                    return 1;
                }
                else
                {
                    var dot0 = Vector2.Dot(vector, dir0);
                    var dot1 = Vector2.Dot(vector, dir1);

                    if (cross0 == 0 && cross1 == 0)
                    {
                        if (dot0 > dot1)
                        {
                            return -1;
                        }
                        else
                        {
                            return 1;
                        }
                    }
                    else if (cross1 == 0)
                    {
                        if (dot1 < 0)
                        {
                            return -1;
                        }
                        else
                        {
                            if (cross0 > 0)
                            {
                                return 1;
                            }
                            else
                            {
                                return -1;
                            }
                        }
                    }
                    else if (cross0 == 0)
                    {
                        if (dot0 < 0)
                        {
                            return 1;
                        }
                        else
                        {
                            if (cross1 > 0)
                            {
                                return -1;
                            }
                            else
                            {
                                return 1;
                            }
                        }
                    }
                    else if (cross0 > 0)
                    {
                        if (dot0 > dot1)
                        {
                            return -1;
                        }
                        else
                        {
                            return 1;
                        }
                    }
                    else
                    {
                        if (dot0 > dot1)
                        {
                            return 1;
                        }
                        else
                        {
                            return -1;
                        }
                    }

                }*/


            };
            searchPoints.Sort(sort);

            bool searchSuccess = false;
            for(int i = 0; i < searchPoints.Count;i++)
            {
                var point = searchPoints[i];
                if(point == p0)
                {
                    continue;
                }

                if(IsConnect(points, p0, point,edges))
                {
                    if(p0 != start && point == start)
                    {
                        searchSuccess = false;
                        break;
                    }
                    else
                    {
                        searchPoints.RemoveAt(i);
                        res.Add(point);
                        vector = point - p0;
                        p0 = point;
                        searchSuccess = true;
                        break;
                    }
                }
            }

            if(!searchSuccess)
            {
                break;
            }

        }

        return res;

    }

    public static List<Vector2> BallConcave(List<Vector2> points)
    {
        BallConcave ballConcave = new BallConcave(points);
        return ballConcave.GetConcaveBall(2);
    }

    private static List<int> connectIndexs = new List<int>();
    private static bool IsConnect(List<Vector2> points, Vector2 point0, Vector2 point1, List<Vector2Int> edges)
    {
        int index0 = points.IndexOf(point0);
        int index1 = points.IndexOf(point1);
        if(index0 == -1 || index1 == -1)
        {
            Debug.LogError("Connect Index Error");
        }
        Vector2Int edge0 = new Vector2Int(index0, index1);
        Vector2Int edge1 = new Vector2Int(index1, index0);

        if (edges.Contains(edge0) || edges.Contains(edge1))
        {
            return true;
        }

        //共线的情况
        connectIndexs.Clear();
        for (int i = 0; i < edges.Count; i++)
        {
            var edge = edges[i];
            if(edge.x == index0)
            {
                connectIndexs.Add(edge.y);
            }
            else if(edge.y == index0)
            {
                connectIndexs.Add(edge.x);
            }
        }

        for (int i = 0; i < connectIndexs.Count; i++)
        {
            var pos = points[connectIndexs[i]];
            var dir0 = (pos - point0).normalized;
            var dir1 = (point1 - point0).normalized;
            if(dir0 == dir1)
            {
                return true;
            }
        }

        connectIndexs.Clear();
        for (int i = 0; i < edges.Count; i++)
        {
            var edge = edges[i];
            if (edge.x == index1)
            {
                connectIndexs.Add(edge.y);
            }
            else if (edge.y == index1)
            {
                connectIndexs.Add(edge.x);
            }
        }

        for (int i = 0; i < connectIndexs.Count; i++)
        {
            var pos = points[connectIndexs[i]];
            var dir0 = (pos - point1).normalized;
            var dir1 = (point0 - point1).normalized;
            if (dir0 == dir1)
            {
                return true;
            }
        }

        return false;
    }
    #endregion
}

public class BallConcave
{
    struct Point2dInfo : IComparable<Point2dInfo>
    {
        public Vector2 Point;
        public int Index;
        public float DistanceTo;
        public Point2dInfo(Vector2 p, int i, float dis)
        {
            this.Point = p;
            this.Index = i;
            this.DistanceTo = dis;
        }
        public int CompareTo(Point2dInfo other)
        {
            return DistanceTo.CompareTo(other.DistanceTo);
        }
        public override string ToString()
        {
            return Point + "," + Index + "," + DistanceTo;
        }
    }
    public BallConcave(List<Vector2> list)
    {
        this.points = list;
        //points.Sort();
        flags = new bool[points.Count];
        for (int i = 0; i < flags.Length; i++)
            flags[i] = false;
        InitDistanceMap();
        InitNearestList();
    }
    private bool[] flags;
    private List<Vector2> points;
    private float[,] distanceMap;
    private List<int>[] rNeigbourList;
    private void InitNearestList()
    {
        rNeigbourList = new List<int>[points.Count];
        for (int i = 0; i < rNeigbourList.Length; i++)
        {
            rNeigbourList[i] = GetSortedNeighbours(i);
        }
    }
    private void InitDistanceMap()
    {
        distanceMap = new float[points.Count, points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            for (int j = 0; j < points.Count; j++)
            {
                distanceMap[i, j] = GetDistance(points[i], points[j]);
            }
        }
    }
    public float GetRecomandedR()
    {
        float r = float.MinValue;
        for (int i = 0; i < points.Count; i++)
        {
            if (distanceMap[i, rNeigbourList[i][1]] > r)
                r = distanceMap[i, rNeigbourList[i][1]];
        }
        return r;
    }
    public float GetMinEdgeLength()
    {
        float min = float.MaxValue;
        for (int i = 0; i < points.Count; i++)
        {
            for (int j = 0; j < points.Count; j++)
            {
                if (i < j)
                {
                    if (distanceMap[i, j] < min)
                        min = distanceMap[i, j];
                }
            }
        }
        return min;
    }
    public List<Vector2> GetConcaveBall(float radius)
    {
        List<Vector2> ret = new List<Vector2>();
        List<int>[] adjs = GetInRNeighbourList(2 * radius);
        ret.Add(points[0]);
        //flags[0] = true;
        int i = 0, j = -1, prev = -1;
        while (true)
        {
            j = GetNextPointBallPivoting(prev, i, adjs[i], radius);
            if (j == -1)
                break;
            Vector2 p = BallConcave.GetCircleCenter(points[i], points[j], radius);
            ret.Add(points[j]);
            flags[j] = true;
            prev = i;
            i = j;
        }
        return ret;
    }
    public List<Vector2> GetConcaveEdge(float radius)
    {
        List<Vector2> ret = new List<Vector2>();
        List<int>[] adjs = GetInRNeighbourList(2 * radius);
        ret.Add(points[0]);
        int i = 0, j = -1, prev = -1;
        while (true)
        {
            j = GetNextPointEdgePivoting(prev, i, adjs[i], radius);
            if (j == -1)
                break;
            //Vector2 p = BallConcave.GetCircleCenter(points[i], points[j], radius);
            ret.Add(points[j]);
            flags[j] = true;
            prev = i;
            i = j;
        }
        return ret;
    }
    private bool CheckValid(List<int>[] adjs)
    {
        for (int i = 0; i < adjs.Length; i++)
        {
            if (adjs[i].Count < 2)
            {
                return false;
            }
        }
        return true;
    }
    public bool CompareAngel(Vector2 a, Vector2 b, Vector2 m_origin, Vector2 m_dreference)
    {

        Vector2 da = new Vector2(a.x - m_origin.x, a.y - m_origin.y);
        Vector2 db = new Vector2(b.x - m_origin.x, b.y - m_origin.y);
        float detb = GetCross(m_dreference, db);

        // nothing is less than zero degrees
        if (detb == 0 && db.x * m_dreference.x + db.y * m_dreference.y >= 0) return false;

        float deta = GetCross(m_dreference, da);

        // zero degrees is less than anything else
        if (deta == 0 && da.x * m_dreference.x + da.y * m_dreference.y >= 0) return true;

        if (deta * detb >= 0)
        {
            // both on same side of reference, compare to each other
            return GetCross(da, db) > 0;
        }

        // vectors "less than" zero degrees are actually large, near 2 pi
        return deta > 0;
    }
    public int GetNextPointEdgePivoting(int prev, int current, List<int> list, float radius)
    {
        if (list.Count == 2 && prev != -1)
        {
            return list[0] + list[1] - prev;
        }
        Vector2 dp;
        if (prev == -1)
            dp = new Vector2(1, 0);
        else
            dp = points[prev] - points[current];
        int min = -1;
        for (int j = 0; j < list.Count; j++)
        {
            if (!flags[list[j]])
            {
                if (min == -1)
                {
                    min = list[j];
                }
                else
                {
                    Vector2 t = points[list[j]];
                    if (CompareAngel(points[min], t, points[current], dp) && GetDistance(t, points[current]) < radius)
                    {
                        min = list[j];
                    }
                }
            }
        }
        //main.ShowMessage("seek P" + points[min].Index);
        return min;
    }
    public int GetNextPointBallPivoting(int prev, int current, List<int> list, float radius)
    {
        SortAdjListByAngel(list, prev, current);
        for (int j = 0; j < list.Count; j++)
        {
            if (flags[list[j]])
                continue;
            int adjIndex = list[j];
            Vector2 xianp = points[adjIndex];
            Vector2 rightCirleCenter = GetCircleCenter(points[current], xianp, radius);
            if (!HasPointsInCircle(list, rightCirleCenter, radius, adjIndex))
            {
                return list[j];
            }
        }
        return -1;
    }
    private void SortAdjListByAngel(List<int> list, int prev, int current)
    {
        Vector2 origin = points[current];
        Vector2 df;
        if (prev != -1)
            df = new Vector2(points[prev].x - origin.x, points[prev].y - origin.y);
        else
            df = new Vector2(1, 0);
        int temp = 0;
        for (int i = list.Count; i > 0; i--)
        {
            for (int j = 0; j < i - 1; j++)
            {
                if (CompareAngel(points[list[j]], points[list[j + 1]], origin, df))
                {
                    temp = list[j];
                    list[j] = list[j + 1];
                    list[j + 1] = temp;
                }
            }
        }
    }
    private bool HasPointsInCircle(List<int> adjPoints, Vector2 center, float radius, int adjIndex)
    {
        for (int k = 0; k < adjPoints.Count; k++)
        {
            if (adjPoints[k] != adjIndex)
            {
                int index2 = adjPoints[k];
                if (IsInCircle(points[index2], center, radius))
                    return true;
            }
        }
        return false;
    }
    public static Vector2 GetCircleCenter(Vector2 a, Vector2 b, float r)
    {
        float dx = b.x - a.x;
        float dy = b.y - a.y;
        float cx = 0.5f * (b.x + a.x);
        float cy = 0.5f * (b.y + a.y);
        if (r * r / (dx * dx + dy * dy) - 0.25 < 0)
        {
            return new Vector2(-1, -1);
        }
        float sqrt = Mathf.Sqrt(r * r / (dx * dx + dy * dy) - 0.25f);
        return new Vector2(cx - dy * sqrt, cy + dx * sqrt);
    }
    public static bool IsInCircle(Vector2 p, Vector2 center, float r)
    {
        float dis2 = (p.x - center.x) * (p.x - center.x) + (p.y - center.y) * (p.y - center.y);
        return dis2 < r * r;
    }
    public List<int>[] GetInRNeighbourList(float radius)
    {
        List<int>[] adjs = new List<int>[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            adjs[i] = new List<int>();
        }
        for (int i = 0; i < points.Count; i++)
        {

            for (int j = 0; j < points.Count; j++)
            {
                if (i < j && distanceMap[i, j] < radius)
                {
                    adjs[i].Add(j);
                    adjs[j].Add(i);
                }
            }
        }
        return adjs;
    }
    private List<int> GetSortedNeighbours(int index)
    {
        List<Point2dInfo> infos = new List<Point2dInfo>(points.Count);
        for (int i = 0; i < points.Count; i++)
        {
            infos.Add(new Point2dInfo(points[i], i, distanceMap[index, i]));
        }
        infos.Sort();
        List<int> adj = new List<int>();
        for (int i = 1; i < infos.Count; i++)
        {
            adj.Add(infos[i].Index);
        }
        return adj;
    }
    public static float GetDistance(Vector2 p1, Vector2 p2)
    {
        return Mathf.Sqrt((p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y));
    }
    public static float GetCross(Vector2 a, Vector2 b)
    {
        return a.x * b.y - a.y * b.x;
    }
}
