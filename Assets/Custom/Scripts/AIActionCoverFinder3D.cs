using System;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using TD.Public;
using UnityEngine;
using UnityEngine.Serialization;

public class AIActionCoverFinder3D : AIAction
{
    private CharacterMovement _characterMovement;
    private CharacterPathfinder3D _characterPathfinder3D;

    public override void Initialization()
    {
        if (!ShouldInitialize) return;
        base.Initialization();
        _characterMovement = GetComponentInParent<Character>()?.FindAbility<CharacterMovement>();
        _characterPathfinder3D = GetComponentInParent<Character>()?.FindAbility<CharacterPathfinder3D>();

        if (_characterPathfinder3D == null)
        {
            Debug.LogWarning(this.name +
                             " : the AIActionPathfinderToCover3D AI Action requires the CharacterPathfinder3D ability");
        }
    }

    public override void PerformAction()
    {
        if (_brain.CoverData.SelectedCrouchPoint == null)
        {
            _characterMovement?.SetHorizontalMovement(0f);
            _characterMovement?.SetVerticalMovement(0f);
            return;
        };
        
        _characterPathfinder3D.SetNewDestination(_brain.CoverData.SelectedCrouchPoint);
    }

    public override void OnExitState()
    {
        base.OnExitState();

        _characterPathfinder3D?.SetNewDestination(null);
        _characterMovement?.SetHorizontalMovement(0f);
        _characterMovement?.SetVerticalMovement(0f);
    }
}