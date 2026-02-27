using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Dictionary<AmmoTypes, int> WeaponAmmo = new();
    public List<Weapon> Weapons;

    void Start()
    {
        WeaponAmmo.Add(AmmoTypes.LightAmmo, 10);
        WeaponAmmo.Add(AmmoTypes.ShotgunAmmo, 12);

        Weapons.Add(new Weapon(0, "LBM-4", 8, 40, 47, "Pistol", AmmoTypes.LightAmmo));
        Weapons.Add(new Weapon(1, "RMX-7", 6, 24, 10, "Shotgun", AmmoTypes.ShotgunAmmo));
    }

    void Update()
    {
        
    }
}
