using UnityEngine;

public enum AmmoTypes { 
LightAmmo,
ShotgunAmmo
};

public class Weapon : MonoBehaviour
{
    public int WeaponId;
    public string WeaponName;
    public int CurrentAmmo;
    public int MaxAmmo;
    public float AttackRate;
    public float Damage;
    public float AttackRange;
    public string WeaponType;
    public AmmoTypes AmmoType;

    public Weapon(int weaponId, string weaponName, int maxAmmo, float attackRate, float damage, float attackRange, string weaponType, AmmoTypes ammoType)
    {
        WeaponId = weaponId;
        WeaponName = weaponName;
        MaxAmmo = maxAmmo;
        CurrentAmmo = MaxAmmo;
        AttackRate = attackRate;
        Damage = damage;
        AttackRange = attackRange;
        WeaponType = weaponType;
        AmmoType = ammoType;
    }

}
