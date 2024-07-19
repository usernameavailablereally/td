using System;
using System.Collections.Generic;
using TD.Public;
using UnityEngine;

public class CoverGroup : MonoBehaviour
{
    [SerializeField] private float _sphereRadius = 0.2f;
    
    private const int MaxCoversExpected = 12;
    private List<Cover> _covers = new(MaxCoversExpected);

    private int _lastChildCount = -1;

    private void OnDrawGizmos()
    {
        UpdateCachedCovers();
        for (var i = 0; i < _covers.Count; i++)
        {
            Cover cover = _covers[i];
            
            for (var j = 0; j < cover.CrouchTransforms.Length; j++)
            {
                Transform crouchPoint = cover.CrouchTransforms[j];
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(crouchPoint.position, _sphereRadius);
            }
        }
    }

    private void UpdateCachedCovers()
    {
        if (transform.childCount == _lastChildCount)
        {
            return;
        }
        
        _lastChildCount = transform.childCount;
        _covers.Clear();
        GetComponentsInChildren(_covers);
    }
}