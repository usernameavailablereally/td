using System.Collections.Generic;
using UnityEngine;

public class ComparerByDistanceToPoint<T> : IComparer<T> where T: Component
{
    public Vector3 Point { get; set; }
    public int Compare(T a, T b)
    {
        return Vector3.Distance(Point, a.transform.position)
            .CompareTo(Vector3.Distance(Point, b.transform.position));
    }
}