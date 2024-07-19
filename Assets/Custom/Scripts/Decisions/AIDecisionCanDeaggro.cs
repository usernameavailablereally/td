using MoreMountains.Tools;

public class AIDecisionCanDeaggro : AIDecision
{
    public float MinTimeInState = 3f;

    protected float _targetTime;

    public override bool Decide()
    {
        return EvaluateTime();
    }

    protected virtual bool EvaluateTime()
    {
        if (_brain == null)
        {
            return false;
        }

        return (_brain.TimeInThisState >= _targetTime) && _brain.Detection.AggroAmount < 0.01f;
    }

    public override void Initialization()
    {
        base.Initialization();
        ResetTime();
    }

    public override void OnEnterState()
    {
        base.OnEnterState();
        ResetTime();
    }

    protected virtual void ResetTime()
    {
        _targetTime = MinTimeInState;
    }
}