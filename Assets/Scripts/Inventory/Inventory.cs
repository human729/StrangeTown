using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Dictionary<AmmoTypes, int> WeaponAmmo = new();
    public static List<Weapon> Weapons = new();

    void Start()
    {
        WeaponAmmo.Add(AmmoTypes.LightAmmo, 60);
        WeaponAmmo.Add(AmmoTypes.ShotgunAmmo, 12);
    }

    void Update()
    {
        
    }
}
