using System;

public class GameEvent<T>
{
    public event Action<T> OnTriggered;
    
    public virtual void Trigger(T param)
    {
        OnTriggered?.Invoke(param);
    }
}