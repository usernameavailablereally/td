using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class ThrowableWeapon : Weapon
{
    [SerializeField] private WeaponAim _aim;
    [SerializeField] private ProjectileTrajectory _trajectory;
    [SerializeField] private ThrowableItem _prefab;
    [SerializeField] private Transform _view;
    [SerializeField] private float _shotCooldown = 2f;
    public Vector3 SpawnOffset;

    protected MMSimpleObjectPooler _objectPool;
    private bool _isAiming;
    private bool _isThrowing;
    private Vector3 _targetPosition;
    private float _lastShootTime;
    private WeaponAim.AimControls _lastAimControl;

    public override bool IsReloadNeeded() => !_isThrowing;

    public override void Initialization()
    {
        base.Initialization();
        _objectPool = gameObject.GetComponent<MMSimpleObjectPooler>();
        _lastShootTime = Mathf.NegativeInfinity;
        _trajectory.Hide();
    }

    public override void WeaponInputStart()
    {
        base.WeaponInputStart();

        bool isOnCooldown = _lastShootTime >= Time.time - _shotCooldown;

        if (isOnCooldown)
        {
            return;
        }

        _lastShootTime = Time.time;
        _isAiming = true;
        _trajectory.Show();
        _targetPosition = _aim.GetReticlePosition();
    }

    public override void WeaponInputStop()
    {
        base.WeaponInputStop();

        if (!_isAiming)
        {
            return;
        }

        _targetPosition = _aim.GetReticlePosition();
        _isAiming = false;
        _isThrowing = true;
        _lastAimControl = _aim.AimControl;
        _aim.AimControl = WeaponAim.AimControls.Off;
    }

    public override void WeaponInputReleased()
    {
        base.WeaponInputReleased();

        if (!_isThrowing)
        {
            return;
        }

        _isThrowing = false;
        Spawn();
        _trajectory.Hide();
        _aim.AimControl = _lastAimControl;
    }

    public override void ShootRequest()
    {
        // base isn't called intentionally
    }

    protected override void Update()
    {
        base.Update();

        if (!_isAiming)
        {
            return;
        }

        var newSpawnWorldPosition = transform.position;
        newSpawnWorldPosition += SpawnOffset;
        _targetPosition = _aim.GetReticlePosition();
        Vector3 direction =
            _trajectory.CalculateForceWithAngle(newSpawnWorldPosition, _targetPosition, 1f);
        _trajectory.UpdateTrajectory(newSpawnWorldPosition, _targetPosition, direction);
    }

    protected virtual void Spawn()
    {
        var newSpawnWorldPosition = transform.position;
        newSpawnWorldPosition += SpawnOffset;
        Vector3 direction = _trajectory.CalculateForceWithAngle(newSpawnWorldPosition, _targetPosition, 1f);

        GameObject nextGameObject = _objectPool.GetPooledGameObject();
        ThrowableItem item = nextGameObject.MMGetComponentNoAlloc<ThrowableItem>();

        nextGameObject.transform.position = newSpawnWorldPosition;
        item.SetOwner(Owner.gameObject);
        nextGameObject.gameObject.SetActive(true);

        item.AddForce(direction, ForceMode.Impulse);
    }

    public void SetViewPose(Transform rightHand)
    {
        _view.position = rightHand.transform.position;
        _view.right = rightHand.transform.forward;
    }
}