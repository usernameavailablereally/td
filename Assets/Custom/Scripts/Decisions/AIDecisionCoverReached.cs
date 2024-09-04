using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace TD
{
    public class AIDecisionCoverReached : AIDecisionReached
    {
        protected override Vector3 GetTargetPosition()
        {
            return _brain.CoverData.SelectedCrouchPoint.position;
        }

        protected override bool IsDecisionPossible()
        {
            return _brain.CoverData.SelectedCrouchPoint != null;
        }

        public override bool Decide()
        {
            bool result =  base.Decide();
            _brain.CoverData.IsReached = result;
            return result;
        }
    }
}