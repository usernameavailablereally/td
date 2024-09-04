using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class AIDecisionNoiseReached : AIDecisionReached
{
    private CharacterMovement _movement;

    public override void Initialization()
    {
        base.Initialization();
        _movement = this.gameObject.GetComponentInParent<Character>()?.FindAbility<CharacterMovement>();
    }

    protected override Vector3 GetTargetPosition()
    {
        return _brain.Target.position;
    }

    protected override bool IsDecisionPossible()
    {
        return _brain.Target != null;
    }

    public override bool Decide()
    {
        bool result = base.Decide();
        
        if (result)
        {
            _movement.SetMovement(Vector2.zero);
        }

        return result;
    }
}