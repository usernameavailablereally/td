using UnityEngine;
using System.Linq;

namespace MoreMountains.TopDownEngine
{	
	/// <summary>
	/// Add this component to a character and it'll be able to make noise
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/Abilities/Character Noise")] 
	public class CharacterNoise : CharacterAbility
	{	
		/// This method is only used to display a helpbox text at the beginning of the ability's inspector
		public override string HelpBoxText() { return "This component allows your character to make noise"; }

		[Tooltip("the object we want")]
		public SphereCollider NoiseCollider; 
		[Tooltip("the frequency (in seconds) at which to check for obstacles")]
		public float TargetCheckFrequency = 1f;
		[Tooltip("Maximum noise value")]
		public float MaxNoise; 
		[Tooltip("Noisy value factor")]
		public float NoisyValueFactor; 
		[Tooltip("Alarm value factor")]
		public float AlarmValueFactor; 
		
		public CharacterStates.MovementStates[] SilentStates;
		public CharacterStates.MovementStates[] NoisyStates;
		public CharacterStates.MovementStates[] AlarmStates;
		public NoiseQualifier[] NoisesQualifier;
		
		[System.Serializable]
		public class NoiseQualifier
		{
			public CharacterStates.MovementStates MovementState;
			public float NoiseFactor;
		}
		
		private float _initialNoiseRadius;
 

		protected override void OnEnable()
		{
			base.OnEnable();
			_initialNoiseRadius = NoiseCollider.radius;
		}
		public override void ProcessAbility()
		{
			base.ProcessAbility();
			UpdateNoise();
		}
		protected float _lastTargetCheckTimestamp = 0f;
		private void UpdateNoise()
		{

			foreach (CharacterHandleWeapon handleWeapon in _handleWeaponList)
			{
				if (handleWeapon.CurrentWeapon != null)
				{
					if (handleWeapon.CurrentWeapon.WeaponState.CurrentState == Weapon.WeaponStates.WeaponStart ||
					    handleWeapon.CurrentWeapon.WeaponState.CurrentState == Weapon.WeaponStates.WeaponUse)
					{
						SetNoiseLevel(AlarmValueFactor);
						return;
					}
				}
			}
			
			// we check if there's a need to detect a new target
			if (Time.time - _lastTargetCheckTimestamp < TargetCheckFrequency)
			{
				return;
			} 

			_lastTargetCheckTimestamp = Time.time;

			NoiseQualifier noiseQualifier = NoisesQualifier.First(qualifier => qualifier.MovementState == _movement.CurrentState);
			{
				SetNoiseLevel(noiseQualifier.NoiseFactor);
			}
		}

		private void SetNoiseLevel(float level)
		{
			Debug.Log($"SetNoiseLevel {level}");
			NoiseCollider.enabled = level > 0;
			float targetValue = _initialNoiseRadius * _characterMovement.MovementSpeed * level;
			NoiseCollider.radius = Mathf.Clamp(targetValue, 0, MaxNoise);
			GUIManager.Instance.UpdateNoiseBar(NoiseCollider.radius, 0f, MaxNoise, _character.PlayerID);
		}
	}
}