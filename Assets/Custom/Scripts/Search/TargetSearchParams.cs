using MoreMountains.TopDownEngine;
using UnityEngine;

public struct TargetSearchParams
{
    // Mandatory params
    public Vector3 TargetPosition;
    public Vector3 SearchStart;
    public GameObject SearchOwner;

    public LayerMask TargetLayerMask;
    public LayerMask ObstacleMask;
    public bool SearchTargetComponentInParent;
    public float MaxDistanceFromTarget;

    // Optional params
    public float Radius;
    public bool IncludeFarCovers;
        
    public TargetSearchParams(Vector3 targetPosition, Vector3 searchStart, GameObject brainOwner)
    {
        SearchStart = searchStart;
        TargetPosition = targetPosition;
        SearchOwner = brainOwner;
            
        Radius = Mathf.Infinity;
        IncludeFarCovers = true;
        TargetLayerMask = LayerManager.PlayerLayerMask;
        ObstacleMask = LayerManager.ObstaclesLayerMask;
        SearchTargetComponentInParent = false;
        MaxDistanceFromTarget = Mathf.Infinity;
    }
}