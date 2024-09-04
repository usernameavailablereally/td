using MoreMountains.InventoryEngine;
using MoreMountains.TopDownEngine;
using Unity.Netcode;
using UnityEngine;
using MoreMountains.FeedbacksForThirdParty;

public class PlayerNetworkController : NetworkBehaviour
{
    public CustomTextFeedbackVisualizer customTextFeedbackVisualizer;
    public NetworkVariable<float> horizontalMovement = new NetworkVariable<float>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> verticalMovement = new NetworkVariable<float>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<CharacterStates.MovementStates> characterMovementState = new NetworkVariable<CharacterStates.MovementStates>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public NetworkVariable<Vector3> position = new NetworkVariable<Vector3>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<Vector3> rotation = new NetworkVariable<Vector3>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<Vector3> weaponAim = new NetworkVariable<Vector3>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> health = new NetworkVariable<float>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public float maxPositionDeviation = 0.3f;

    private TopDownController3D _controller;
    private CharacterMovement _characterMovement;
    private CharacterJump3D _characterJump;
    private Health _health;
    private CharacterCrouch _characterCrouch;
    private CharacterRun _characterRun;
    private CharacterHandleWeapon _characterHandleWeapon;
    private InputManager _inputManager;
    private Character _character;
    private CharacterInventory _characterInventory;

    private Inventory _inventoryMain;
    private Inventory _inventoryWeapon;
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        customTextFeedbackVisualizer.Initialize();
        var spawnPointsNumber = (int)NetworkObjectId % LevelManager.Instance.InitialSpawnPoints.Count;
        this.transform.position = LevelManager.Instance.InitialSpawnPoints[1].transform.position;
        _character = GetComponent<Character>();
        _controller = GetComponent<TopDownController3D>();
        _characterJump = GetComponent<CharacterJump3D>();
        _characterCrouch = GetComponent<CharacterCrouch>();
        _characterRun = GetComponent<CharacterRun>();
        _characterHandleWeapon = GetComponent<CharacterHandleWeapon>();
        _characterInventory = GetComponent<CharacterInventory>();
        _health = GetComponent<Health>();

        InventoryCharacterIdentifier _characterInventoryIndetifier = GetComponent<InventoryCharacterIdentifier>();

        _characterMovement = GetComponent<CharacterMovement>();


        string PlayerID = "Player1";
        string NetworkPlayerID = "NetworkPlayer" + NetworkObjectId;
        
        if (IsOwner)
        {
            _character.CharacterType = Character.CharacterTypes.Player;
            _inputManager = _character.LinkedInputManager;
            LevelManager.Instance.Players.Add(_character);
            MMCameraEvent.Trigger(MMCameraEventTypes.SetTargetCharacter, _character);
            MMCameraEvent.Trigger(MMCameraEventTypes.StartFollowing);
            _character.MovementState.OnStateChange += OnMovementStateChange;
            _characterInventoryIndetifier.PlayerID = PlayerID;

            _characterHandleWeapon.OnWeaponChange += () =>
            {
                var weaponID = _characterHandleWeapon.CurrentWeapon ? _characterHandleWeapon.CurrentWeapon.WeaponID : "";
                ChangeWeaponRpc(weaponID);
            };

            _characterHandleWeapon.OnShootStart += () => TriggerShootStartRpc();
            _characterHandleWeapon.OnShootStop += () => TriggerShootStopRpc();
            _characterHandleWeapon.OnReload += () => TriggerReloadRpc();

        }
        else
        {
            PlayerID = NetworkPlayerID;
            _characterMovement.ScriptDrivenInput = true;
            _character.CharacterType = Character.CharacterTypes.AI;
            _character.SetInputManager(null);
            _characterMovement.SetHorizontalMovement(0f);
            _characterMovement.SetVerticalMovement(0f);
            _character.ResetInput();

            _characterInventoryIndetifier.PlayerID = PlayerID;

            _inventoryMain = GameObject.Find(_characterInventory.MainInventoryName).AddComponent<Inventory>();
            _inventoryMain.PlayerID = PlayerID;
            _inventoryMain.Content = new InventoryItem[24];
            _inventoryMain.SetOwner(gameObject);

            _inventoryWeapon = GameObject.Find(_characterInventory.WeaponInventoryName).AddComponent<Inventory>();
            _inventoryWeapon.PlayerID = PlayerID;
            _inventoryWeapon.Content = new InventoryItem[1];
            _inventoryWeapon.SetOwner(gameObject);
            _inventoryWeapon.InventoryType = Inventory.InventoryTypes.Equipment;
        }

        if (IsServer)
        {
            health.Value = _health.CurrentHealth;
            _health.OnHit += () => health.Value = _health.CurrentHealth;
            _health.OnDeath += TriggerDeathRpc;
        } else {
            _health.DamageDisabled();
        }

        health.OnValueChanged += (float prev, float current) =>
        {
            var healthDelta = prev - current;
            if (healthDelta > 0) {
                customTextFeedbackVisualizer.VisualizeDamage(healthDelta.ToString());
            }
        };
        _character.PlayerID = PlayerID;
        _character.name = PlayerID;

        _characterInventory.PlayerID = PlayerID;
    }


    public override void OnDestroy()
    {
        if (IsOwner)
        {
            TriggerDeathRpc();
        }
    }

    public override void OnNetworkDespawn()
    {
        _health.OnDeath -= TriggerDeathRpc;
        _character.SetInputManager(null);
        MMCameraEvent.Trigger(MMCameraEventTypes.StopFollowing);
        MMCameraEvent.Trigger(MMCameraEventTypes.SetTargetCharacter, null);
        if (IsOwner)
        {
            var AutoFocus = FindObjectOfType<MMAutoFocus>();
            AutoFocus.FocusTargets = new Transform[0];
            AutoFocus.FocusTargetID = 0;
            LevelManager.Instance.ResetPlayers();
            var players = FindObjectsByType<Character>(FindObjectsSortMode.None);
            foreach(var player in players)
            {
                Destroy(player);
            }
        }
        Destroy(this);
    }

    private void UpdateOwner()
    {
        horizontalMovement.Value = _inputManager.PrimaryMovement.x;
        verticalMovement.Value = _inputManager.PrimaryMovement.y;
        position.Value = gameObject.transform.position;

        _characterMovement.SetMovement(new Vector2(horizontalMovement.Value, verticalMovement.Value));
        weaponAim.Value = _characterHandleWeapon.WeaponAimComponent ? _characterHandleWeapon.WeaponAimComponent.CurrentAim : Vector3.zero;
        _health.SetHealth(health.Value);
    }

    private void OnMovementStateChange()
    {
        characterMovementState.Value = _character.MovementState.CurrentState;
    }

    private void UpdateClients()
    {
        _characterMovement.SetMovement(new Vector2(horizontalMovement.Value, verticalMovement.Value));
        float positionDeviation = Vector3.Distance(gameObject.transform.position, position.Value);
        if (positionDeviation > maxPositionDeviation)
        {
            _controller.MovePosition(position.Value);
        }
        if (characterMovementState.Value == CharacterStates.MovementStates.Jumping)
            _characterJump.JumpStart();
        else
            _characterJump.JumpStop();

        if (characterMovementState.Value == CharacterStates.MovementStates.Crouching || characterMovementState.Value == CharacterStates.MovementStates.Crawling)
            _characterCrouch.StartForcedCrouch();
        else
            _characterCrouch.StopForcedCrouch();

        if (characterMovementState.Value == CharacterStates.MovementStates.Running)
            _characterRun.RunStart();
        else if (characterMovementState.Value != CharacterStates.MovementStates.Jumping)
            _characterRun.RunStop();

        if (weaponAim.Value != Vector3.zero)
        {
            _characterHandleWeapon.WeaponAimComponent.enabled = false;
            _characterHandleWeapon.ForceWeaponAimControl = true;
            _characterHandleWeapon.ForcedWeaponAimControl = WeaponAim.AimControls.Script;
            _characterHandleWeapon.WeaponAimComponent.SetCurrentAim(weaponAim.Value);

        } else
        {
            _characterHandleWeapon.ForceWeaponAimControl = false;
            _characterHandleWeapon.ForcedWeaponAimControl = WeaponAim.AimControls.PrimaryMovement;
        }
    }

    [Rpc(SendTo.Server)]
    public void TriggerShootStartRpc() => ShootStartRpc();
    [Rpc(SendTo.Server)]
    public void TriggerShootStopRpc() => ShootStopRpc();
    [Rpc(SendTo.Server)]
    public void TriggerReloadRpc() => ReloadRpc();

    [Rpc(SendTo.Server)]
    public void TriggerDeathRpc() => NetworkObject.Despawn();

    [Rpc(SendTo.Server)]
    public void ChangeWeaponRpc(string weaponID, RpcParams rpcParams = default) => UpdatePlayerWeaponRpc(weaponID, rpcParams);

    [Rpc(SendTo.NotOwner)]
    void ShootStartRpc() => _characterHandleWeapon.ShootStart();

    [Rpc(SendTo.NotOwner)]
    void ShootStopRpc() => _characterHandleWeapon.ShootStop();

    [Rpc(SendTo.NotOwner)]
    void ReloadRpc() => _characterHandleWeapon.Reload();

    [Rpc(SendTo.ClientsAndHost)]
    void UpdatePlayerWeaponRpc(string weaponID, RpcParams rpcParams = default)
    {
        if (weaponID == "")
        {
            if (_characterHandleWeapon.CurrentWeapon)
            {
                var index = 0;
                foreach (InventoryItem item in _inventoryWeapon.Content)
                {
                    if (item && item.ItemID == _characterHandleWeapon.CurrentWeapon.WeaponID)
                    {
                        _inventoryWeapon.UnEquipItem(item, index);
                        break;
                    }
                    index++;
                }
            }
        }
        else
        {
            if (!_characterHandleWeapon.CurrentWeapon || weaponID != _characterHandleWeapon.CurrentWeapon.WeaponID)
            {
                var index = 0;
                foreach (InventoryItem item in _inventoryMain.Content)
                {
                    if (item && item.ItemID == weaponID)
                    {
                        item.TargetEquipmentInventoryName = _characterInventory.WeaponInventoryName;
                        _inventoryMain.EquipItem(item, index);
                        break;
                    }
                    index++;
                }
            }
        }
    }

    public void Update()
    {
        if (!IsOwner)
        {
            UpdateClients();
        }
        else
        {
            UpdateOwner();
        }
    }
}
