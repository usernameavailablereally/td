using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Custom.Scripts.Decisions
{
    public class AIActionAggroAtTarget : AIAction
    {
        [SerializeField] private AIDecisionLineOfSightToTarget3DCustom _lineOfSight;
        [SerializeField] private AggroView _view;
        
        private float _amount;

        public override void PerformAction()
        {
            bool targetVisible = _lineOfSight.LastResult;
            _brain.Detection.UpdateAggro(Time.deltaTime, targetVisible);
            _view.SetValue(_brain.Detection.AggroAmount01);
        }

        public override void OnEnterState()
        {
            _brain.Detection.Reset();
            _view.SetValue(_brain.Detection.AggroAmount01);
        }

        public override void OnExitState()
        {
            _brain.Detection.Reset();
            _view.SetValue(_brain.Detection.AggroAmount01);
        }
    }
}