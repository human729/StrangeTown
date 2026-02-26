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

    void Start()
    {
        
    }

    void Update()
    {
        
    }

}
