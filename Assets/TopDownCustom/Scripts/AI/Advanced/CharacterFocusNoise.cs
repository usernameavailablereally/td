using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MoreMountains.TopDownEngine
{	
	/// <summary>
	/// Add this component to a character and it'll be able to make noise AND focus on specific target
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/Abilities/Character Focus Noise")] 
	public class CharacterFocusNoise : CharacterAbility
	{	
		/// This method is only used to display a helpbox text at the beginning of the ability's inspector
		public override string HelpBoxText() { return "This component allows your character to make noise and focus other bots on specific target"; }

		[Tooltip("Collider of the Focus Noise area")]
		public FocusNoiseArea FocusNoiseArea; 
		
		/*[Tooltip("Visualization transform of the noise area")]
		public Transform NoiseVisialization; */
		[Tooltip("the frequency (in seconds) at which to check for obstacles")]
		public float TargetCheckFrequency = 1f;
		 
		 [Tooltip("Maximum noise value")]
		public float AINoiseAreaFactor;

		protected override void Awake()
		{
			base.Awake();
			FocusNoiseArea.FocusZoneCollider.enabled = false;
		}
		/*public NoiseQualifier[] NoisesQualifier;*/
		
 
		
		/*[System.Serializable]
		public class NoiseQualifier
		{
			public CharacterStates.MovementStates MovementState;
			public float NoiseFactor;
		}*/
		
		
		/*
		private float _initialNoiseRadius;*/
 

		/*protected override void OnEnable()
		{
			base.OnEnable();
			//_initialNoiseRadius = NoiseCollider.radius;
		}*/

		/*protected override void Initialization()
		{
			base.Initialization();
			//SetNoiseLevel(0);
		}*/

		public override void ProcessAbility()
		{
			base.ProcessAbility();
			/*UpdateNoise();*/
			UpdateFocusZone();
		}
		protected float _lastTargetCheckTimestamp = 0f;
		private void UpdateNoise()
		{
		}

		private void UpdateFocusZone()
		{
			if (Time.time - _lastTargetCheckTimestamp < TargetCheckFrequency)
			{
				return;
			} 

			_lastTargetCheckTimestamp = Time.time;
			
			if (_character.CharacterBrain.CurrentState.StateName == "Destroying")
			{
				FocusNoiseArea.FocusZoneCollider.enabled = true;
				FocusNoiseArea.FocusZoneCollider.radius = AINoiseAreaFactor;
			}
			else
			{
				FocusNoiseArea.FocusZoneCollider.enabled = false;
				FocusNoiseArea.FocusZoneCollider.radius = 0;
			}
		}

		/*private void SetNoiseLevel(float level)
		{
			NoiseCollider.enabled = level > 0;
			float targetValue = _initialNoiseRadius * _characterMovement.MovementSpeed * level;
			NoiseCollider.radius = Mathf.Clamp(targetValue, 0, MaxNoise);
			Vector3 currentVisualizationScale = NoiseVisialization.localScale;
			currentVisualizationScale.x = currentVisualizationScale.z = NoiseCollider.radius * 2;
			NoiseVisialization.localScale = currentVisualizationScale;
			GUIManager.Instance.UpdateNoiseBar(NoiseCollider.radius, 0f, MaxNoise, _character.PlayerID);
		}*/

		/*public void SetAbilityState(bool isActive)
		{
			_isActive = isActive;
		}*/
	}
}