using System;
using UnityEngine;

public class FrameSkipper : MonoBehaviour
{
    [SerializeField] private int _updateFrameRate = 15;

    protected float _minFrameDuration;
    protected float _lastFrameTime;
    
    public bool FrameSkipped { get; private set; }

    public event Action OnNormalFrame;

    public void Awake()
    {
        _minFrameDuration = 1f / _updateFrameRate;
        _lastFrameTime = -1;
    }

    public void Update()
    {
        FrameSkipped = Time.time - _lastFrameTime < _minFrameDuration;
        
        if (!FrameSkipped)
        {
            _lastFrameTime = Time.time;
            OnNormalFrame?.Invoke();    
        }
    }
}