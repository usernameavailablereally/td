using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public class AIDecisionDetectTargetAlways : AIDecision
{
    [Tooltip("the layer(s) to search our target on")]
    public LayerMask TargetLayerMask;

    protected Collider[] _hits;
    protected Collider _collider;

    public override void Initialization()
    {
        _collider = this.gameObject.GetComponentInParent<Collider>();
        _hits = new Collider[1];
    }

    public override bool Decide()
    {
        int numberOfCollidersFound = Physics.OverlapSphereNonAlloc(_collider.bounds.center, Mathf.Infinity, _hits, TargetLayerMask);

        if (numberOfCollidersFound < 1)
        {
            return false;   
        }

        _brain.Target = _hits[0].transform;
        return true;
    }
}
