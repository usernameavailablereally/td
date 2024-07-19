using UnityEngine;

namespace TD.Public
{
    public class DetectionState
    {
        private readonly AIAAggroConfig _config;

        public float AggroAmount { get; private set; }
        public float AggroAmount01 => Mathf.Clamp01(AggroAmount / _config.AggroToTriggerAttack);
        public bool AggroTresholdReached => AggroAmount > _config.AggroToTriggerAttack;

        public DetectionState(AIAAggroConfig aggroConfig)
        {
            _config = aggroConfig;
        }

        public void UpdateAggro(float duration, bool trigger)
        {
            float modifier = trigger ? _config.AggroPerSec : -_config.DeAggroPerSec;
            AggroAmount = Mathf.Clamp(AggroAmount + modifier * duration, 0, Mathf.Infinity);
        }

        public void Reset()
        {
            AggroAmount = 0;
        }
    }
}