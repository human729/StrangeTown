using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Dictionary<AmmoTypes, int> WeaponAmmo = new();
    public List<Weapon> Weapons;
    public List<GameObject> Items;

    void Awake()
    {
        WeaponAmmo.Add(AmmoTypes.LightAmmo, 10);
        WeaponAmmo.Add(AmmoTypes.ShotgunAmmo, 12);

        Weapons.Add(new Weapon(0, "LBM-4", 8, 40, damage: 47, 100f, "Pistol", AmmoTypes.LightAmmo));
        Weapons.Add(new Weapon(1, "RMX-7", 6, 24, damage: 10, 50f, "Shotgun", AmmoTypes.ShotgunAmmo));
    }

    void Update()
    {
        
    }
}
