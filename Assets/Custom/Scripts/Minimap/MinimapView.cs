using System;
using MoreMountains.Tools;
using UnityEngine;

public class MinimapView : MonoBehaviour
{
    protected MMSimpleObjectPooler _objectPool;

    private void Awake()
    {
        _objectPool = gameObject.GetComponent<MMSimpleObjectPooler>();
    }

    public void CreateView()
    {
        
    }
}