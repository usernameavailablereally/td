using System;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class ThrowableItem : TopDownMonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private GameObject Owner;

    public void SetOwner(GameObject owner)
    {
        Owner = owner;
    }

    public void AddForce(Vector3 force, ForceMode forceMode)
    {
        _rb.AddForce(force, forceMode);
    }
}