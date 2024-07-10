using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// This action forces the character to crouch if it can
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/AI/Actions/AIActionCrouchStart")]
	//[RequireComponent(typeof(CharacterCrouch))]
	public class AIActionCrouchStart : AIAction
	{
		protected CharacterCrouch _characterCrouch;
		protected Character _character;
		protected CharacterHandleWeapon _handleWeapon;

		/// <summary>
		/// Grabs dependencies
		/// </summary>
		public override void Initialization()
		{
			if(!ShouldInitialize) return;
			base.Initialization();
			_character = this.gameObject.GetComponentInParent<Character>();
			_characterCrouch = _character?.FindAbility<CharacterCrouch>();
			_handleWeapon = gameObject.GetComponentInParent<Character>()?.FindAbility<CharacterHandleWeapon>();            
		}

		/// <summary>
		/// On PerformAction we crouch
		/// </summary>
		public override void PerformAction()
		{
			if ((_character == null) || (_characterCrouch == null))
			{
				return;
			}

			if ((_character.MovementState.CurrentState != CharacterStates.MovementStates.Crouching)
			    && (_character.MovementState.CurrentState != CharacterStates.MovementStates.Crawling))
			{
				_characterCrouch.StartForcedCrouch();
			}
		}
	}
}