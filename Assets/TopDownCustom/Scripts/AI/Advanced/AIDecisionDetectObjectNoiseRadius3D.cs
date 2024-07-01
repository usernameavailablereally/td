using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// This decision will return true if an object on its TargetLayer layermask is within its specified radius, false otherwise. It will also set the Brain's Target to that object.
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/AI/Decisions/AIDecisionDetectObjectNoiseRadius3D")]
	//[RequireComponent(typeof(Character))]
	
	public class AIDecisionDetectObjectNoiseRadius3D : AIDecisionDetectNoiseRadius3D
	{
		
		[Tooltip("If the noise shource is found, stop triggering")]
		public bool IsStopTriggeringEnemiesIfFound;

		protected override void ProcessFoundTransform(Transform noiseSource)
		{
			Debug.Log($" ProcessFoundTransform {noiseSource.name} IsStopTriggeringEnemiesIfFound = {IsStopTriggeringEnemiesIfFound}");
			if (IsStopTriggeringEnemiesIfFound)
			{
				noiseSource.GetComponent<Collider>().enabled = false;
			}
		}
	}
}