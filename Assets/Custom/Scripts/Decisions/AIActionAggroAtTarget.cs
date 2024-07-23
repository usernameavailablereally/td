using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Custom.Scripts.Decisions
{
    public class AIActionAggroAtTarget : AIAction
    {
        [SerializeField] private AIDecisionLineOfSightToTarget3DCustom _lineOfSight;
        [SerializeField] private AggroView _view;
        [SerializeField] private float _maxViewDistance = 25f;
        
        private float _amount;

        public override void PerformAction()
        {
            bool targetVisible = _lineOfSight.LastResult;
            float distanceCorrelation = GetDistanceCorrelation();
            _brain.Detection.UpdateAggro(Time.deltaTime, targetVisible, distanceCorrelation);
            _view.SetValue(_brain.Detection.AggroAmount01);
        }

        private float GetDistanceCorrelation()
        {
            if (_brain.Target == null)
            {
                return 0;
            }

            float distanceToTarget = Vector3.Magnitude(_brain.Target.position - transform.position);
            return Mathf.Clamp01(distanceToTarget / _maxViewDistance);
        }

        public override void OnEnterState()
        {
            _brain.Detection.IfTriggeredReduceToNearTriggered();
            _view.SetValue(_brain.Detection.AggroAmount01);
        }

        public override void OnExitState()
        {
            _view.SetValue(_brain.Detection.AggroAmount01);
        }
    }
}