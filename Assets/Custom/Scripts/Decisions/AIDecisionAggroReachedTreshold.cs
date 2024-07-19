using MoreMountains.Tools;

public class AIDecisionAggroReachedTreshold : AIDecision
{
    public override bool Decide()
    {
        return _brain.Detection.AggroTresholdReached;
    }
}
