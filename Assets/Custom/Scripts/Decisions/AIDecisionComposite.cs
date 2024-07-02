using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

[System.Serializable]
public class AIConditionsList : MMReorderableArray<AIDecisionCondition>
{
}

public class AIDecisionComposite : AIDecision
{
    [MMReorderableAttribute(null, "Condition", null)]
    [SerializeField] private AIConditionsList _conditions;

    public override bool Decide()
    {
        foreach (AIDecisionCondition condition in _conditions)
        {
            if (condition.Decision.LastResult != condition.ExpectedResult)
            {
                return false;
            }
        }

        return true;
    }
}
