using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class AIStatesVisualizer : MonoBehaviour
{
    /// the feedback to play when getting damage
    [Tooltip("the feedback to play when getting damage")]
    public MMFeedbacks StatusFeedback;
    public AIBrain AIBrain;
    public float DelayBeetweenStates;
    private readonly Queue<string> _statusesPool = new Queue<string>();
    private float _timeSinceLastStateShow;
    

    private string _storedState = string.Empty;

    private void Awake()
    {
        StatusFeedback?.Initialization(this.gameObject);
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
       // FeedbackText = StatusFeedback.Feedbacks[0].;
        //FeedbackText.TargetText = newState;
        //FeedbackText.Play();
        StatusFeedback?.PlayFeedbacks(this.transform.position, 1);    
    }

    public string DequeueNextStatus()
    {
        if (_statusesPool.Count == 0)
        {
         //   return AIBrain.CurrentState.StateName;
        }
        
        return _statusesPool.Dequeue();
    }
}
