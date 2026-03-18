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
        DisableButtons();
        UseButton.SetActive(true);
        DropButton.SetActive(true);
    }

    public void DisableButtons()
    {
        GameObject ItemsContainer = GameObject.Find("ItemsContainer");

        foreach (Image Item in ItemsContainer.GetComponentsInChildren<Image>())
        {
            foreach (Button button in Item.GetComponentsInChildren<Button>())
            {
                button.gameObject.SetActive(false);
            }
        }
    }

    public void DropItem()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        //ItemGameObject.SetActive(true);
        //ItemGameObject.transform.position = player.transform.position + Vector3.down;
        GameObject SpawnedItem = Instantiate(item.Prefab, player.transform.position + player.transform.forward, Quaternion.Euler(-90f, 0f, 0f));
        SpawnedItem.SetActive(true);
        Inventory Inventory = player.GetComponent<Inventory>();
        Inventory.Items.Remove(gameObject);

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
