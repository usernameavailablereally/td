using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class AIActionStopMovement : AIAction
{
    private CharacterMovement _movement;

    public override void Initialization()
    {
        base.Initialization();
        _movement = this.gameObject.GetComponentInParent<Character>()?.FindAbility<CharacterMovement>();
    }

    public override void PerformAction()
    {
        _movement.SetMovement(Vector2.zero);
    }
}