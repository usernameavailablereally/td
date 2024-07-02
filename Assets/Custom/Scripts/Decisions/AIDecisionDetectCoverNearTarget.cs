using System;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using TD.Public;
using UnityEngine;
using UnityEngine.Serialization;

namespace TD
{
    public class AIDecisionDetectCoverNearTarget : AIDecision
    {
        [field: SerializeField] float _radius = 3f;
        [field: SerializeField] float _frequency = 5f;
        [FormerlySerializedAs("_mxDistanceFromPlayer")] [FormerlySerializedAs("_MmxDistanceFromPlayer")] [FormerlySerializedAs("_MaxDistanceFromPlayer")] [field: SerializeField] float _maxDistanceFromPlayer = 15f;
        [field: SerializeField] LayerMask _targetLayerMask = LayerManager.PlayerLayerMask;

        private float timeToDecide;

        private void Update()
        {
            timeToDecide -= Time.deltaTime;
        }

        public override bool Decide()
        {
            if (timeToDecide > 0)
            {
                return LastResult;
            }

            timeToDecide += _frequency;
            return DecideInternal();
        }

        private bool DecideInternal()
        {
            if (_brain.Target == null) return FailSearch();

            TargetSearchParams searchParams = new(_brain.Target.position, transform.position, _brain.Owner)
            {
                Radius = _radius,
                TargetLayerMask = _targetLayerMask,
                SearchTargetComponentInParent = true,
                MaxDistanceFromTarget = _maxDistanceFromPlayer
            };

            if (!CoversManager.Instance.GetBestCover(searchParams, out Cover cover))
            {
                return FailSearch();
            }

            Vector3 targetPosition = _brain.Target.position;
            _brain.CoverData.CurrentCover = cover;
            _brain.CoverData.SelectedCrouchPoint = Utils.GetBestCrouchPoint(targetPosition, cover);
            _brain.CoverData.IsReached = false;
            return true;
        }

        private bool FailSearch()
        {
            _brain.CoverData.CurrentCover = null;
            _brain.CoverData.SelectedCrouchPoint = null;
            return false;
        }

        protected virtual void OnDrawGizmosSelected()
        {
            if (_brain != null && _brain.CoverData.CurrentCover != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(_brain.CoverData.SelectedCrouchPoint.position, 0.2f);

                Gizmos.color = Color.red;
                foreach (Transform point in _brain.CoverData.CurrentCover.CrouchTransforms)
                {
                    if (point == _brain.CoverData.SelectedCrouchPoint)
                    {
                        continue;
                    }

                    Gizmos.DrawWireSphere(point.position, 0.2f);
                }
            }

            if (_brain != null && _brain.Target != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(_brain.Target.position, _radius);
            }
        }
    }
}