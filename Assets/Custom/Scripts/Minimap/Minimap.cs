using System;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    [SerializeField] private MinimapView _view;
    [SerializeField] private bool _spawnPrimitiveTarget;

    public void Awake()
    {
        GameEvents.Instance.OnPlayerSpawned.OnTriggered += HandlePlayerSpawned;
        GameEvents.Instance.OnPlayerDeath.OnTriggered += HandlePlayerDeath;
    }

    private void Start()
    {
        if (_spawnPrimitiveTarget)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "Primitive Cube Target for map";
            GameEvents.Instance.OnPlayerSpawned.Trigger(new PlayerSpawnParams(cube.transform, null, false));
        }
    }

    private void HandlePlayerDeath(Transform player)
    {
        _view.HideView(player);
    }

    public void OnDestroy()
    {
        GameEvents.Instance.OnPlayerSpawned.OnTriggered -= HandlePlayerSpawned;
        GameEvents.Instance.OnPlayerDeath.OnTriggered -= HandlePlayerDeath;
    }

    private void HandlePlayerSpawned(PlayerSpawnParams spawn)
    {
        _view.ShowPlayer(spawn.Player, spawn.IsOwner, spawn.Character);
    }
}