using MoreMountains.Tools;
using UnityEngine;

[System.Serializable]
public class AIDecisionCondition
{
    [field: SerializeField] public AIDecision Decision { get; private set; }
    [field: SerializeField] public bool ExpectedResult { get; private set; }
}