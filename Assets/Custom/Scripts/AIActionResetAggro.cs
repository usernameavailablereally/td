using MoreMountains.Tools;
using UnityEngine;

public class AIActionResetAggro : AIAction
{
    [SerializeField] private AggroView _view;

    public override void OnEnterState()
    {
        base.OnEnterState();
        _brain.Detection.Reset();
        _view.SetValue(_brain.Detection.AggroAmount01);
    }

    public override void PerformAction()
    {
        
    }
}