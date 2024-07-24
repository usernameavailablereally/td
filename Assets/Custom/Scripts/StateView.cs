using System;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Serialization;

public class StateView : MonoBehaviour
{
    [SerializeField] private AIBrain _brain;
    [SerializeField] private Transform _detectionMask;
    [SerializeField] private GameObject _detectionView;
    [SerializeField] private GameObject _destroyingView;
    [SerializeField] private GameObject _seekingView;

    private List<GameObject> _views;
    private GameObject _lastView;
    private string _lastState;

    protected void Awake()
    {
        _views = new List<GameObject>()
        {
            _detectionView,
            _destroyingView,
            _seekingView
        };

        for (int i = 0; i < _views.Count; i++)
        {
            GameObject view = _views[i];
            view.SetActive(false);
        }
    }

    protected void Update()
    {
        string currentState = _brain.CurrentState?.StateName;

        if (currentState != null && currentState != _lastState)
        {
            OnStateChanged(currentState);
            _lastState = currentState;
        }
    }

    private void OnStateChanged(string stateName)
    {
        GameObject newView = GetViewForState(stateName);
        SetActiveView(newView);
    }

    private void SetActiveView(GameObject newView)
    {
        if (newView != null)
        {
            newView.SetActive(true);
        }

        if (_lastView != null && _lastView != newView)
        {
            _lastView.SetActive(false);
        }

        _lastView = newView;
    }

    public void SetDetectionValue(float value)
    {
        Vector3 scale = _detectionMask.localScale;
        scale.y = value;
        _detectionMask.localScale = scale;
    }

    private GameObject GetViewForState(string stateName)
    {
        return stateName switch
        {
            "LookingAtTarget" => _detectionView,
            "SeekingPlayer" => _seekingView,
            "SeekingNoise" => _seekingView,
            "Destroying" => _destroyingView,
            "SeekingCoverNearTarget" => _destroyingView,
            "ReloadOrDestroy" => _destroyingView,
            "DestroyPlainOrFromCover" => _destroyingView,
            "Covered" => _destroyingView,
            "Reload" => _destroyingView,
            _ => null
        };
    }
}