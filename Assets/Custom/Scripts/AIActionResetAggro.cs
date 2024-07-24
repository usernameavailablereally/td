using MoreMountains.Tools;
using UnityEngine;

public class AIActionResetAggro : AIAction
{
    [SerializeField] private StateView _view;

    public override void OnEnterState()
    {
        base.OnEnterState();
        _brain.Detection.Reset();
        _view.SetDetectionValue(_brain.Detection.AggroAmount01);
    }

    public override void PerformAction()
    {
        
    }
}