using MoreMountains.Tools;
using UnityEngine;

namespace TD
{
    public class AIDecisionCoverReached : AIDecision
    {
        [SerializeField] private float _distanceTreshold = 0.5f;

        // public override void Initialization()
        // {
        //     base.Initialization();
        // }

        public override bool Decide()
        {
            if (_brain.CoverData.SelectedCrouchPoint == null)
            {
                return false;
            }

            var tresholdSqr = _distanceTreshold * _distanceTreshold;
            var distanceSqr = Vector3.SqrMagnitude(_brain.CoverData.SelectedCrouchPoint.position - transform.position);
            bool result =  distanceSqr < tresholdSqr;
            _brain.CoverData.IsReached = result;
            return result;
        }
    }
}