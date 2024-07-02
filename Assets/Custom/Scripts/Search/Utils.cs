using System.Collections.Generic;
using TD.Public;
using UnityEngine;

namespace TD
{
    public static class Utils
    {
        private static TargetFinder<Cover> _targetFinder = new();

        public static bool GetBestCover(TargetSearchParams searchParams, out Cover closest)
        {
            return _targetFinder.Find(searchParams, out closest);
        }
        
        public static bool GetBestCover(TargetSearchParams searchParams, List<Cover> covers, out Cover closest)
        {
            return _targetFinder.Find(searchParams, covers, out closest);
        }
        
        public static Transform GetBestCrouchPoint(Vector3 targetPosition, Cover cover)
        {
            Vector3 targetToCoverDir = cover.Position - targetPosition;
            targetToCoverDir.y = 0;
            targetToCoverDir.Normalize();
            Transform bestPoint = null;
            float closestAngleDiff = Mathf.Infinity;
            const float bestPossibleDot = 1;
        
            for (int index = 0; index < cover.CrouchTransforms.Length; index++)
            {
                Transform point = cover.CrouchTransforms[index];
                Vector3 coverToPointDir = point.position - cover.Position;
                coverToPointDir.y = 0;
                coverToPointDir.Normalize();
                float angleDiff = Mathf.Abs(bestPossibleDot - Vector3.Dot(targetToCoverDir, coverToPointDir));
                // Debug.Log($"{angleDiff}   {Vector3.Dot(targetToCoverDir, coverToPointDir)}");
                Debug.DrawLine(cover.Position, point.position, Color.yellow);

                if (bestPoint == null || angleDiff < closestAngleDiff)
                {
                    bestPoint = point;
                    closestAngleDiff = angleDiff;
                }
            }
        
            Debug.DrawLine(targetPosition, cover.Position, Color.black);
            Debug.DrawLine(cover.Position, bestPoint.position, Color.black);

            return bestPoint;
        }
    }
}