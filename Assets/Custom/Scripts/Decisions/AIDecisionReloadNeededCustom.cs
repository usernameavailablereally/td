using MoreMountains.Tools;
using MoreMountains.TopDownEngine;

public class AIDecisionReloadNeededCustom : AIDecision
{
    protected CharacterHandleWeapon _characterHandleWeapon;

    public override void Initialization()
    {
        base.Initialization();
        _characterHandleWeapon = this.gameObject.GetComponentInParent<Character>()?.FindAbility<CharacterHandleWeapon>();
    }

    public override bool Decide()
    {
        if (_characterHandleWeapon == null)
        {
            return false;
        }

        if (_characterHandleWeapon.CurrentWeapon == null)
        {
            return false;
        }

        return _characterHandleWeapon.CurrentWeapon.IsReloadNeeded();
    }
}