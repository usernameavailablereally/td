using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace TD
{
    public class AIDecisionCoverReached : AIDecision
    {
        [SerializeField] private float _distanceTreshold = 0.9f;
        [SerializeField] private float _noPathTreshold = 1.5f;

        private CharacterPathfinder3D _characterPathfinder3D;

        public override void Initialization()
        {
            base.Initialization();
            _characterPathfinder3D = GetComponentInParent<Character>()?.FindAbility<CharacterPathfinder3D>();
        }

        public override bool Decide()
        {
            if (_brain.CoverData.SelectedCrouchPoint == null)
            {
                return false;
            }
            
            float tresholdSqr = _distanceTreshold * _distanceTreshold;
            float distanceSqr = Vector3.SqrMagnitude(_brain.CoverData.SelectedCrouchPoint.position - transform.position);
            bool result =  distanceSqr < tresholdSqr;

            // agent is stuck, possibly accept position as reached with bigger treshold
            if (_characterPathfinder3D.NextWaypointIndex < 0)
            {
                float noPathTresholdSqr = _noPathTreshold * _noPathTreshold;
                result = distanceSqr < noPathTresholdSqr;
                // ToDo: handle case when we're stuck and above treshold
            }
            
            _brain.CoverData.IsReached = result;
            return result;
        }
    }
}