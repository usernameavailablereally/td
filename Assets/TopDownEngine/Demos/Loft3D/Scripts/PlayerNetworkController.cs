using MoreMountains.InventoryEngine;
using MoreMountains.TopDownEngine;
using Unity.Netcode;
using UnityEngine;
using MoreMountains.FeedbacksForThirdParty;
using Unity.Collections;
using MoreMountains.Feedbacks;
using UnityEngine.Serialization;

[GenerateSerializationForType(typeof(byte))]
public class PlayerNetworkController : NetworkBehaviour
{
    public CustomTextFeedbackVisualizer customTextFeedbackVisualizer;
    public NicknameController nicknameController;
    public NetworkVariable<float> horizontalMovement = new NetworkVariable<float>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> verticalMovement = new NetworkVariable<float>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<CharacterStates.MovementStates> characterMovementState = new NetworkVariable<CharacterStates.MovementStates>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public NetworkVariable<Vector3> position = new NetworkVariable<Vector3>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<Vector3> rotation = new NetworkVariable<Vector3>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<FixedString32Bytes> weaponCurrent = new NetworkVariable<FixedString32Bytes>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public NetworkVariable<Vector3> weaponAim = new NetworkVariable<Vector3>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> health = new NetworkVariable<float>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<FixedString32Bytes> PlayerNickname = new NetworkVariable<FixedString32Bytes>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    
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
        if (IsOwner || position.Value == Vector3.zero)
        {
            var spawnPointsNumber = (int)NetworkObjectId % LevelManager.Instance.InitialSpawnPoints.Count;
            this.transform.position = LevelManager.Instance.InitialSpawnPoints[spawnPointsNumber].transform.position;
        } else
        {
            this.transform.position = position.Value;
        }

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

        string NetworkPlayerID = "NetworkPlayer" + NetworkObjectId;
        string PlayerID = IsOwner ? "Player1" : NetworkPlayerID;

        _character.PlayerID = PlayerID;
        _character.name = PlayerID;

        _characterInventory.PlayerID = PlayerID;

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
                weaponCurrent.Value = weaponID;
            };

            _characterHandleWeapon.OnShootStart += () => TriggerShootStartRpc();
            _characterHandleWeapon.OnShootStop += () => TriggerShootStopRpc();
            _characterHandleWeapon.OnReload += () => TriggerReloadRpc();

            weaponCurrent.Value = new FixedString32Bytes(_characterHandleWeapon.InitialWeapon.name);
            PlayerNickname.Value = nicknameController.LoadCachedNicknameFromStorage();
        }
        else
        {
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
            if ((_characterInventory.AutoPickItems.Length > 0))
            {
                foreach (AutoPickItem item in _characterInventory.AutoPickItems)
                {
                    _inventoryMain.AddItem(item.Item, item.Quantity);
                }
            }

            _inventoryWeapon = GameObject.Find(_characterInventory.WeaponInventoryName).AddComponent<Inventory>();
            _inventoryWeapon.PlayerID = PlayerID;
            _inventoryWeapon.Content = new InventoryItem[1];
            _inventoryWeapon.SetOwner(gameObject);
            _inventoryWeapon.InventoryType = Inventory.InventoryTypes.Equipment;

            _characterHandleWeapon.InitialWeapon = null;

            MMF_Player[] feedbackPlayers = GetComponentsInChildren<MMF_Player>(true);
            foreach (var feedbackPlayer in feedbackPlayers)
            {
                if (feedbackPlayer.name == "DamageFeedback")
                {
                    foreach (var feedback in feedbackPlayer.FeedbacksList)
                    {
                        if (feedback.Label == "Flash") feedback.Active = false;
                    }
                }
                
            }

            UpdatePlayerWeapon(weaponCurrent.Value.ToString());
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


        weaponCurrent.OnValueChanged += (FixedString32Bytes prev, FixedString32Bytes next) => UpdatePlayerWeapon(next.ToString());

        PlayerNickname.OnValueChanged += (FixedString32Bytes prev, FixedString32Bytes next) =>
        {
            nicknameController.SetNickname(next.ToString());
        };

        nicknameController.SetNickname(PlayerNickname.Value.ToString());
    }

    public override void OnDestroy()
    {
        if (IsOwner)
        {
            _character.SetInputManager(null);
            MMCameraEvent.Trigger(MMCameraEventTypes.StopFollowing);
            MMCameraEvent.Trigger(MMCameraEventTypes.SetTargetCharacter, null);
            var AutoFocus = FindObjectOfType<MMAutoFocus>();
            if (AutoFocus)
            {
                AutoFocus.FocusTargets = new Transform[0];
                AutoFocus.FocusTargetID = 0;
            }
        } else {
            foreach (var inventory in _inventoryMain.GetComponents<Inventory>())
            {
                if (inventory.PlayerID == _character.PlayerID)
                {
                    Destroy(inventory);
                }
            }

            foreach (var inventory in _inventoryWeapon.GetComponents<Inventory>())
            {
                if (inventory.PlayerID == _character.PlayerID)
                {
                    Destroy(inventory);
                }
            }

        }
    }

    public override void OnNetworkDespawn()
    {
        _health.OnDeath -= TriggerDeathRpc;
        _health.Kill();
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
            if (_characterHandleWeapon.WeaponAimComponent) {
                _characterHandleWeapon.WeaponAimComponent.enabled = false;
                _characterHandleWeapon.WeaponAimComponent.SetCurrentAim(weaponAim.Value);
            }
            _characterHandleWeapon.ForceWeaponAimControl = true;
            _characterHandleWeapon.ForcedWeaponAimControl = WeaponAim.AimControls.Script;

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
    public void TriggerDeathRpc() => DieRpc();

    [Rpc(SendTo.NotOwner)]
    void ShootStartRpc() => _characterHandleWeapon.ShootStart();

    [Rpc(SendTo.NotOwner)]
    void ShootStopRpc() => _characterHandleWeapon.ShootStop();

    [Rpc(SendTo.NotOwner)]
    void ReloadRpc() => _characterHandleWeapon.Reload();

    [Rpc(SendTo.NotServer)]
    void DieRpc() => _health.Kill();

    void UpdatePlayerWeapon(string weaponID)
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
                        return;
                    }
                    index++;
                }
            }
        }
        else
        {
            if (_characterHandleWeapon.CurrentWeapon && _characterHandleWeapon.CurrentWeapon.WeaponID == weaponID)
            {
                return;
            }
               
            var index = 0;
            foreach (InventoryItem item in _inventoryMain.Content)
            {
                if (item && item.ItemID == weaponID)
                {
                    item.TargetEquipmentInventoryName = _characterInventory.WeaponInventoryName;
                    _inventoryMain.EquipItem(item, index);
                    if (_characterHandleWeapon.CurrentWeapon)
                    {
                        MMF_Player[] feedbackPlayers = _characterHandleWeapon.CurrentWeapon.GetComponentsInChildren<MMF_Player>(true);
                        foreach (var feedbackPlayer in feedbackPlayers)
                        {
                            foreach (var feedback in feedbackPlayer.FeedbacksList)
                            {
                                if (feedback.Label == "Flash" || feedback.Label == "Cinemachine Impulse")
                                {
                                    feedback.Active = false;
                                }
                            }
                        }
                    }
                        
                    return;
                }
                index++;
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
