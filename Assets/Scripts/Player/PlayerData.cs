using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerData : MonoBehaviour, IDamageable
{
    [Header("UI")]
    [SerializeField] private Text AmmoData;
    [SerializeField] private Text WeaponData;
    [SerializeField] private Text InventoryAmmo;

    [Header("Player data")]
    public float Health;
    public float NaxHealth;
    private bool isAlive;

    [Header("Weapon")]
    private Weapon CurrentWeapon;
    [SerializeField] private Inventory Inventory;

    private bool canShoot = false;

    void Start()
    {
        isAlive = true;
        CurrentWeapon = Inventory.Weapons[0];
        AmmoData.text = $"{CurrentWeapon.CurrentAmmo} / {CurrentWeapon.MaxAmmo}";
        WeaponData.text = $"{CurrentWeapon.WeaponName}";
        InventoryAmmo.text = $"{Inventory.WeaponAmmo[CurrentWeapon.AmmoType]}";
    }

    void Update()
    {
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
            SwitchWeaponUp();
        if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
            SwitchWeaponDown();

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }

        if (Input.GetMouseButtonDown(0) && canShoot)
        {
            Shoot();
        }

        if (Input.GetMouseButtonDown(1) && CurrentWeapon.CurrentAmmo > 0)
        {
            canShoot = true;
        }

        if (Input.GetMouseButtonUp(1)) {
            canShoot = false;
        }
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
        AmmoData.text = $"{CurrentWeapon.CurrentAmmo} / {CurrentWeapon.MaxAmmo}";
        if (CurrentWeapon.CurrentAmmo == 0)
        {
            canShoot = false;
        }
    }

    public void Reload()
    {
        if (Inventory.WeaponAmmo[CurrentWeapon.AmmoType] == 0 || CurrentWeapon.CurrentAmmo == CurrentWeapon.MaxAmmo) return;
        canShoot = false;
        if (CurrentWeapon.CurrentAmmo + Inventory.WeaponAmmo[CurrentWeapon.AmmoType] > CurrentWeapon.MaxAmmo)
        {
            Inventory.WeaponAmmo[CurrentWeapon.AmmoType] -= CurrentWeapon.MaxAmmo - CurrentWeapon.CurrentAmmo;
            CurrentWeapon.CurrentAmmo = CurrentWeapon.MaxAmmo;
        } else
        {
            CurrentWeapon.CurrentAmmo += Inventory.WeaponAmmo[CurrentWeapon.AmmoType];
            Inventory.WeaponAmmo[CurrentWeapon.AmmoType] = 0;
        }
        InventoryAmmo.text = $"{Inventory.WeaponAmmo[CurrentWeapon.AmmoType]}";
        StartCoroutine(ReloadMag());
    }

    private void SwitchWeaponUp()
    {
        if (CurrentWeapon.WeaponId < 1)
        {
            CurrentWeapon = Inventory.Weapons[++CurrentWeapon.WeaponId];
            WeaponData.text = CurrentWeapon.WeaponName;
            AmmoData.text = $"{CurrentWeapon.CurrentAmmo} / {CurrentWeapon.MaxAmmo}";
            InventoryAmmo.text = $"{Inventory.WeaponAmmo[CurrentWeapon.AmmoType]}";
            print($"{Inventory.Weapons.Count}, {CurrentWeapon.WeaponName}");
        }
    }

    private void SwitchWeaponDown()
    {
        if (CurrentWeapon.WeaponId > 0)
        {
            CurrentWeapon = Inventory.Weapons[--CurrentWeapon.WeaponId];
            WeaponData.text = CurrentWeapon.WeaponName;
            AmmoData.text = $"{CurrentWeapon.CurrentAmmo} / {CurrentWeapon.MaxAmmo}";
            InventoryAmmo.text = $"{Inventory.WeaponAmmo[CurrentWeapon.AmmoType]}";
            print($"{Inventory.Weapons.Count}, {CurrentWeapon.WeaponName}");
        }
    }

    IEnumerator ReloadMag()
    {
        yield return new WaitForSeconds(1.5f);
        AmmoData.text = $"{CurrentWeapon.CurrentAmmo} / {CurrentWeapon.MaxAmmo}";
        canShoot = true;
    }
}
