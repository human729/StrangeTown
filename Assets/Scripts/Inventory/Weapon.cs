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
    public string WeaponType;
    public AmmoTypes AmmoType;

    public Weapon(int weaponId, string weaponName, int maxAmmo, float attackRate, float damage, string weaponType, AmmoTypes ammoType)
    {
        WeaponId = weaponId;
        WeaponName = weaponName;
        MaxAmmo = maxAmmo;
        CurrentAmmo = MaxAmmo;
        AttackRate = attackRate;
        Damage = damage;
        WeaponType = weaponType;
        AmmoType = ammoType;
    }

}
