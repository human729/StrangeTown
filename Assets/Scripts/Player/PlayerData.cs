using UnityEngine;
using UnityEngine.UI;

public class PlayerData : MonoBehaviour, IDamageable
{
    [Header("UI")]
    public Text AmmoData;

    [Header("Player data")]
    public float Health;
    public float NaxHealth;
    private bool isAlive;

    [Header("Weapon")]
    public Weapon CurrentWeapon;

    void Start()
    {
        isAlive = true;
        CurrentWeapon = Inventory.Weapons[0];
        AmmoData.text = $"{CurrentWeapon.CurrentAmmo} / {CurrentWeapon.MaxAmmo}";
    }

    void Update()
    {
        
    }

    public bool IsAlive() => isAlive;

    public void TakeDamage(float damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            isAlive = false;
        }
    }

    public void Shoot()
    {
        CurrentWeapon.CurrentAmmo--;
    }

    public void Reload()
    {
        if (Inventory.WeaponAmmo[CurrentWeapon.AmmoType] <= CurrentWeapon.MaxAmmo)
        {
            CurrentWeapon.CurrentAmmo = Inventory.WeaponAmmo[CurrentWeapon.AmmoType];
            Inventory.WeaponAmmo[CurrentWeapon.AmmoType] = 0;
        } else
        {
            CurrentWeapon.CurrentAmmo = CurrentWeapon.MaxAmmo;
            Inventory.WeaponAmmo[CurrentWeapon.AmmoType] -= CurrentWeapon.MaxAmmo;
        }
    }

    public void SwitchWeapon()
    {
        if (Input.GetKeyDown(KeyCode.WheelDown))
        {
            if (CurrentWeapon.WeaponId != 1)
            {
                CurrentWeapon = Inventory.Weapons[CurrentWeapon.WeaponId++];
            }
        }
    }
}
