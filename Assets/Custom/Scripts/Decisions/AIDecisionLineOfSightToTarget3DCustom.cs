using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using TD.Public;
using UnityEngine;
using UnityEngine.Serialization;

namespace MoreMountains.TopDownEngine
{
    public class AIDecisionLineOfSightToTarget3DCustom : AIDecision
    {
        [Tooltip("the layermask to consider as obstacles when trying to determine whether a line of sight is present")]
        public LayerMask LayerMask = LayerManager.ObstaclesLayerMask;

        public List<RaycastLine> _linesToCast;

        protected Collider _collider;

        public override void Initialization()
        {
            _collider = gameObject.GetComponentInParent<Collider>();
        }

        public override bool Decide()
        {
            return CheckLineOfSight();
        }

        protected virtual bool CheckLineOfSight()
        {
            if (_brain.Target == null)
            {
                return false;
            }

            for (int i = 0; i < _linesToCast.Count; i++)
            {
                RaycastLine line = _linesToCast[i];

                if (RaycastWithOffset(line))
                {
                    return true;
                }
            }

            return false;
        }

        private bool RaycastWithOffset(RaycastLine line)
        {
            Vector3 raycastOrigin = _collider.bounds.center + line.OriginOffset;
            Vector3 directionToTarget = (_brain.Target.transform.position + line.ToOffset) - raycastOrigin;

            RaycastHit hit = MMDebug.Raycast3D(
                raycastOrigin,
                directionToTarget.normalized,
                directionToTarget.magnitude,
                LayerMask,
                Color.yellow,
                true);

            Transform hitTransform = hit.collider?.transform;

            Debug.Log($"Hit object ({hitTransform?.name}), expected target ({_brain.Target?.name}), hit object is target's child {hitTransform?.IsChildOf(_brain.Target)}");
            return hitTransform != null &&
                   (hitTransform == _brain.Target || _brain.Target.IsChildOf(hitTransform));
        }
    }
}