using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

// ideally this class should replace all searches in written in monobehaviours 
public class TargetFinder<T> where T: Component
{
    private const int MaxOverlap = 10;
    private const int DefaultTargetsSize = MaxOverlap;

    private readonly ComparerByDistanceToPoint<T> _comparer = new();
    private readonly List<T> _potentialTargets = new(DefaultTargetsSize);
    private readonly Collider[] _hits = new Collider[MaxOverlap];

    public bool Find(TargetSearchParams search, List<T> targets, out T result)
    {
        return FindInternal(search, targets, out result);
    }

    public bool Find(TargetSearchParams search,  out T result)
    {
        _potentialTargets.Clear();
        GetRaycastTargets(search, _potentialTargets);
        return FindInternal(search, _potentialTargets, out result);
    }
    
    private bool FindInternal(TargetSearchParams search, List<T> targets, out T result)
    {
        result = null;

        // float raycastOrigin = _collider.bounds.center + DetectionOriginOffset / 2;

        if (targets.Count == 0)
        {
            return false;
        }

        SortRaycastTargets(search, targets);
        return TryGetOnObscured(search, targets, out result);
    }

    private bool TryGetOnObscured(TargetSearchParams search, List<T> targets, out T result)
    {
        for (int index = 0; index < targets.Count; index++)
        {
            var t = targets[index];
            Vector3 raycastDirection = t.transform.position - search.TargetPosition;

            if (Vector3.SqrMagnitude(raycastDirection) > (search.MaxDistanceFromTarget * search.MaxDistanceFromTarget))
            {
                continue;
            }
            
            RaycastHit hit = MMDebug.Raycast3D(search.SearchStart, raycastDirection, raycastDirection.magnitude,
                search.ObstacleMask, Color.yellow, true);

            if (hit.collider == null)
            {
                result = t;
                return true;
            }
        }

        result = null;
        return false;
    }

    private void SortRaycastTargets(TargetSearchParams search, List<T> targets)
    {
        _comparer.Point = search.SearchStart;
        targets.Sort(_comparer);
    }

    private void GetRaycastTargets(TargetSearchParams search, List<T> potentialTargets)
    {
        Vector3 raycastOrigin = search.SearchStart;
        int numberOfCollidersFound = Physics.OverlapSphereNonAlloc(raycastOrigin, search.Radius, _hits, search.TargetLayerMask);

        if (numberOfCollidersFound == 0)
        {
            return;
        }

        int min = Mathf.Min(MaxOverlap, numberOfCollidersFound);
        
        for (int i = 0; i < min; i++)
        {
            Collider hit = _hits[i];
            if (hit == null)
            {
                continue;
            }

            // eliminate case with self-target
            if (hit.gameObject == search.SearchOwner || hit.transform.IsChildOf(search.SearchOwner.transform))
            {
                continue;
            }

            T target = hit.gameObject.GetComponent<T>();

            if (target == null && search.SearchTargetComponentInParent)
            {
                target = hit.gameObject.GetComponentInParent<T>();
            }
            
            if (target == null)
            {
                continue;
            }

            potentialTargets.Add(target);
        }
    }
}