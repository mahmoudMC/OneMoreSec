using UnityEngine;

public class SpecialWeaponPickup : PickupSystem
{
    [Header("Weapon Settings")]
    [SerializeField]
    private string weaponId = "SpecialWeapon";

    public string WeaponId => weaponId;

    protected override bool CanApplyEffect(GameObject collector)
    {
        // TODO: Weapon System - the pickup should be rejected when the
        // collector already carries this weapon.
        //
        // IWeaponService weaponService =
        //     collector.GetComponentInParent<IWeaponService>();
        //
        // return weaponService != null
        //     && weaponService.CurrentWeaponId != weaponId;

        return true;
    }

    protected override void ApplyEffect(GameObject collector)
    {
        // TODO: Weapon System - replace the player's current weapon.
        // Waiting for the Combat / Weapon System implementation.
        //
        // IWeaponService weaponService =
        //     collector.GetComponentInParent<IWeaponService>();
        //
        // weaponService.EquipWeapon(weaponId);

        Debug.Log(
            $"{PickupName} collected. Waiting for the Weapon System to equip '{weaponId}'.",
            this
        );
    }
}
