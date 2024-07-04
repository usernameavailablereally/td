using System;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class WeaponAttachmentCrouch : MonoBehaviour
{
    [SerializeField] private Transform _normal;
    [SerializeField] private Transform _crouched;
    [SerializeField] private Transform _attachment;
    private MMStateMachine<CharacterStates.MovementStates> _movement;

    protected void Start()
    {
        var character = gameObject.GetComponentInParent<Character>();
        _movement = character.MovementState;
        Debug.Log(character.SendStateChangeEvents);
        _movement.OnStateChange += OnMovementStateChanged;
    }

    protected void OnDestroy()
    {
        _movement.OnStateChange -= OnMovementStateChanged;
    }

    private void OnMovementStateChanged()
    {
        bool isCrouching = _movement.CurrentState == CharacterStates.MovementStates.Crouching;
        _attachment.position = isCrouching ? _crouched.position : _normal.position;
    }
}