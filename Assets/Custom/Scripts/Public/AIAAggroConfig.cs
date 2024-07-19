using UnityEngine;

namespace TD.Public
{
    [CreateAssetMenu]
    public class AIAAggroConfig : ScriptableObject
    {
        [field: SerializeField] public float AggroPerSec { get; private set; } = 20f;
        [field: SerializeField] public float AggroToTriggerAttack { get; private set; } = 60f;
        [field: SerializeField] public float DeAggroPerSec { get; private set; } = 10f;
    }
}