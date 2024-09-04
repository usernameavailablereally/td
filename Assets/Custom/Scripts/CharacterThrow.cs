using System;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using TD.Public;
using UnityEngine;

public class CharacterThrow : CharacterAbility
{
    [SerializeField] private string _throwingAnimationParamName;
    [SerializeField] private Transform _rightHand;
    private CharacterHandleWeapon _characterHandleWeapon;
    private CharacterAnimationEvents _animationEvents;
    private bool _isWaitingForThrowAnimationFrame;
    private int _throwingAnimationParameter;

    public void OnThrowFrameReached()
    {
        if (_isWaitingForThrowAnimationFrame)
        {
            _characterHandleWeapon.CurrentWeapon.WeaponInputReleased();
            _isWaitingForThrowAnimationFrame = false;
        }
    }

    protected override void Initialization()
    {
        base.Initialization();
        _character = gameObject.GetComponentInParent<Character>();
        _characterHandleWeapon = _character?.FindAbility<CharacterHandleWeapon>();
        _animationEvents = _character?.GetComponentInChildren<CharacterAnimationEvents>();
        RegisterAnimatorParameter(_throwingAnimationParamName, AnimatorControllerParameterType.Trigger,
            out _throwingAnimationParameter);
        _animationEvents.OnThrowFrameReached += OnThrowFrameReached;
    }

    private void OnDestroy()
    {
        _animationEvents.OnThrowFrameReached -= OnThrowFrameReached;
    }

    protected override void HandleInput()
    {
        base.HandleInput();

        if (!(_characterHandleWeapon.CurrentWeapon is ThrowableWeapon throwableWeapon))
        {
            return;
        }

        throwableWeapon.SetViewPose(_rightHand);


        if ((_inputManager.ShootButton.State.CurrentState == MMInput.ButtonStates.ButtonDown) ||
            (_inputManager.ShootAxis == MMInput.ButtonStates.ButtonDown))
        {
            _characterHandleWeapon.CurrentWeapon.WeaponInputStart();
        }

        if (((_inputManager.ShootButton.State.CurrentState == MMInput.ButtonStates.ButtonUp) ||
             (_inputManager.ShootAxis == MMInput.ButtonStates.ButtonUp)))
        {
            _characterHandleWeapon.CurrentWeapon.WeaponInputStop();

            bool isWeaponReady = !_characterHandleWeapon.CurrentWeapon.IsReloadNeeded();
            if (isWeaponReady && ! _isWaitingForThrowAnimationFrame)
            {
                _isWaitingForThrowAnimationFrame = true;
                // we don't set state to throwing to allow throw blending with ongoing animation
                MMAnimatorExtensions.UpdateAnimatorTrigger(_animator, _throwingAnimationParameter,
                    _character._animatorParameters, _character.RunAnimatorSanityChecks);
            }
        }
    }
}