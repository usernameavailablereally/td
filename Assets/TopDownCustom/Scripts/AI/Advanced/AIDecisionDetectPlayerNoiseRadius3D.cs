using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// This decision will return true if an object on its TargetLayer layermask is within its specified radius, false otherwise. It will also set the Brain's Target to that object.
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/AI/Decisions/AIDecisionDetectPlayerNoiseRadius3D")]
	//[RequireComponent(typeof(Character))]
	public class AIDecisionDetectPlayerNoiseRadius3D : AIDecisionDetectNoiseRadius3D
	{
	}
}