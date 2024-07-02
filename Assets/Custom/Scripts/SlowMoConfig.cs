using UnityEngine;

namespace Custom
{
    [CreateAssetMenu]
    public class SlowMoConfig : ScriptableObject
    {
        [field: SerializeField] public float SlowMoMeter { get; private set; } = 100f;
        [field: SerializeField] public float SlowMoDrain { get; private set; } = 20f;
        [field: SerializeField] public float SlowMoRestore { get; private set; } = 10f;
        [field: SerializeField] public float MinAmountToStart { get; private set; } = 20f;
        [field: Space]
        [field: SerializeField] public float SlowMoTimeScale { get; private set; } = 0.2f;
        [field: SerializeField] public float EnterLerpSpeed { get; private set; } = 0.8f;
        [field: SerializeField] public float QuitLerpSpeed { get; private set; } = 0.8f;
        [field: SerializeField] public float NormalTimeScale { get; private set; } = 1;
        [field: SerializeField] public float BgMusicPitchFactor { get; private set; } = 0.5f;
    }
}