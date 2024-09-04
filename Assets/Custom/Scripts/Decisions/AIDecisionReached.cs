using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public abstract class AIDecisionReached : AIDecision
{

    [SerializeField] private float _distanceTreshold = 0.6f;
    [SerializeField] private float _noPathTreshold = 1.5f;

    private CharacterPathfinder3D _characterPathfinder3D;

    public override void Initialization()
    {
        base.Initialization();
        _characterPathfinder3D = GetComponentInParent<Character>()?.FindAbility<CharacterPathfinder3D>();
    }

    protected abstract Vector3 GetTargetPosition();
    protected abstract bool IsDecisionPossible();

    public override bool Decide()
    {
        if (!IsDecisionPossible())
        {
            return false;
        }
            
        float tresholdSqr = _distanceTreshold * _distanceTreshold;
        float distanceSqr = Vector3.SqrMagnitude(GetTargetPosition() - transform.position);
        bool result =  distanceSqr < tresholdSqr;

        // agent is stuck, possibly accept position as reached with bigger treshold
        if (_characterPathfinder3D.NextWaypointIndex < 0)
        {
            float noPathTresholdSqr = _noPathTreshold * _noPathTreshold;
            result = distanceSqr < noPathTresholdSqr;
            // ToDo: handle case when we're stuck and above treshold
        }
            
        return result;
    }
}