using System;
using UnityEngine;
using UnityEngine.Events;

public class TriggerArea : MonoBehaviour
{
    public UnityEvent OnTrigger;

    protected void OnTriggerEnter(Collider other)
    {
        OnTrigger?.Invoke();
    }
}