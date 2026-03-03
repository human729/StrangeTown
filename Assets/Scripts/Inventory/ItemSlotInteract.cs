using System;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotInteract : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
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
        print(ItemGameObject.name);
        ItemGameObject.SetActive(true);
        ItemGameObject.transform.position = player.transform.position + Vector3.down;
        
        Destroy(gameObject);
    }

    public void UseItem()
    {
        if (item.Name == "Medicine Kit")
        {
            UseMedicine();
        }

        if (item.Name == "Key")
        {
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
                door.tag = "None";
                door = null;
            }
        }
    }

    private void UseMedicine()
    {
        playerData.Health = playerData.MaxHealth;
    }
}
