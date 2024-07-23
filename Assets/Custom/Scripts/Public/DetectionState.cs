using UnityEngine;

namespace TD.Public
{
    public class DetectionState
    {
        private readonly AIAAggroConfig _config;

        public float AggroAmount { get; private set; }
        public float AggroAmount01 => Mathf.Clamp01(AggroAmount / _config.AggroToTriggerAttack);
        public bool AggroTresholdReached => AggroAmount > _config.AggroToTriggerAttack;

        public bool WasTriggered { get; private set; }

        public DetectionState(AIAAggroConfig aggroConfig)
        {
            _config = aggroConfig;
        }

        public void IfTriggeredReduceToNearTriggered()
        {
            if (!AggroTresholdReached)
            {
                return;
            }

            SetAggro(AggroAmount - Mathf.Min(_config.AggroPerSec, _config.AggroPerSecOnReTrigger));
        }

        public void UpdateAggro(float duration, bool trigger, float distanceCorrelation)
        {
            float correlationCurved = _config.AggroDistanceCurve.Evaluate(distanceCorrelation);
            WasTriggered = WasTriggered || AggroTresholdReached;
            float triggeredModifier = WasTriggered ? _config.AggroPerSecOnReTrigger : _config.AggroPerSec;
            triggeredModifier *= correlationCurved;

            float modifier = trigger ? triggeredModifier : -_config.DeAggroPerSec;
            SetAggro(AggroAmount + modifier * duration);

            WasTriggered = WasTriggered || AggroTresholdReached;
        }

        private void SetAggro(float value)
        {
            AggroAmount = Mathf.Clamp(value, 0, Mathf.Infinity);
        } 

        public void Reset()
        {
            AggroAmount = 0;
        }
    }
}