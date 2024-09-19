using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class GameEvents : MMSingleton<GameEvents>
{
    public GameEvent<PlayerSpawnParams> OnPlayerSpawned { get; private set; } = new();
    public GameEvent<Transform> OnPlayerDeath { get; private set; } = new();
}

public struct PlayerSpawnParams
{
    public Transform Player;
    public bool IsOwner;
    public Character Character;

    public PlayerSpawnParams(Transform player, Character character, bool isOwner)
    {
        Character = character;
        Player = player;
        IsOwner = isOwner;
    }
}