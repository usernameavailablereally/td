using System;
using MoreMountains.Tools;

public class GameEvents : MMSingleton<GameEvents>
{
    public event Action OnPlayerSpawned;

    public void TriggerPlayerSpawned() => OnPlayerSpawned?.Invoke();
}