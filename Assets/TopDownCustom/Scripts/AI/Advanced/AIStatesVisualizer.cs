using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

[RequireComponent(typeof(MMF_Player))]
public class AIStatesVisualizer : MonoBehaviour
{
    public AIBrain AIBrain;
    public float DelayBeetweenStates;
    private readonly Queue<string> _statusesPool = new Queue<string>();
    private float _timeSinceLastStateShow;
    private MMF_Player _statusFeedback;
    

    private string _storedState = string.Empty;

    private void Awake()
    {
        _statusFeedback = GetComponent<MMF_Player>();
        _statusFeedback?.Initialization(this.gameObject);
    }

    private void FixedUpdate()
    {        
        string currentStateName = AIBrain.CurrentState.StateName;
        if (!_storedState.Equals(currentStateName))
        {
            _storedState = currentStateName;
            _statusesPool.Enqueue(currentStateName);
        }
        
        if (Time.time - _timeSinceLastStateShow > DelayBeetweenStates)
        {
            TriggerStateChange();
            _timeSinceLastStateShow = Time.time;
        }
    }

    public void TriggerStateChange()
    {
        _statusFeedback?.PlayFeedbacks(this.transform.position, 1);    
    }

    public string DequeueNextStatus()
    {
        if (_statusesPool.Count == 0)
        {
         return null;
        }
        
        return _statusesPool.Dequeue();
    }
}
