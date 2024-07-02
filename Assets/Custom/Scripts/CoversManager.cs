using System.Collections.Generic;
using MoreMountains.Tools;
using TD.Public;
using UnityEngine;

public class CoversManager : MMSingleton<CoversManager>
{
    public LayerMask CoversLayerMask;

    private List<Cover> _covers;
    protected override void Awake()
    {
        base.Awake();

        _covers = new List<Cover>(FindObjectsOfType<Cover>());
    }

    public bool GetBestCover(TargetSearchParams searchParams, out Cover closest)
    {
        return TD.Utils.GetBestCover(searchParams, out closest);
    }
    
    // take into account line of sight to target
    
    public bool GetBestCoverNoRaycast(TargetSearchParams searchParams, out Cover closest)
    {
        float maxRadiusSqr = searchParams.Radius * searchParams.Radius;
        float clostestDistanceSqr = Mathf.Infinity;
        closest = null;

        for (int index = 0; index < _covers.Count; index++)
        {
            Cover cover = _covers[index];
            float distanceToCoverSqr = Vector3.SqrMagnitude(searchParams.TargetPosition - cover.Position);
            float distanceToAgentSqr = Vector3.SqrMagnitude(searchParams.SearchStart - cover.Position);

            if (distanceToCoverSqr > maxRadiusSqr) continue;
            if (distanceToAgentSqr > clostestDistanceSqr) continue;
            // if (!includeFarCovers)
            // {
            //     
            // }

            closest = cover;
            clostestDistanceSqr = distanceToAgentSqr;
        }

        return closest != null;
    }
}