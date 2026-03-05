using System;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotInteract : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private GameObject UseButton;
    [SerializeField] private GameObject DropButton;
    public Item item;
    public GameObject door;
    public GameObject inventoryMenu;

    public void OnSlotClicked()
    {
        UseButton.SetActive(true);
        DropButton.SetActive(true);
    }

    public void DropItem()
    {
        GameObject ItemGameObject = item.gameObject;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
      
        ItemGameObject.SetActive(true);
        ItemGameObject.transform.position = player.transform.position + Vector3.down;
        Inventory Inventory = player.GetComponent<Inventory>();
        Inventory.Items.Remove(gameObject);
        print($"Dropped {ItemGameObject.name}");

        Destroy(gameObject);
    }

    public void UseItem()
    {
        if (item.Name == "MedicineKit")
        {
            UseMedicine();
        }

        if (item.Name == "Key")
        {
            door = GameObject.FindGameObjectWithTag("Player").GetComponent<PickupItem>().foundDoor;
            UseKey();
            inventoryMenu = GameObject.FindGameObjectWithTag("InventoryUIMenu");
            inventoryMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
        
    }

    public void UseKey()
    {
        if (door != null)
        {
            if (door.GetComponent<Door>().NeedKeyId == item.ItemId)
            {
                print("Success");
                door.GetComponent<Rigidbody>().freezeRotation = false;
                door.tag = "Untagged";
                door = null;
            }
        }
    }

    private void UseMedicine()
    {
        PlayerData playerData = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerData>();
        playerData.Health = playerData.MaxHealth;
    }
}
