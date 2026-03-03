using System;
using UnityEngine;
using UnityEngine.UI;

public class PickupItem : MonoBehaviour
{
    public float PickupRadius;
    public float SphereOffsetMultiplier;
    public float SphereRadius;
    public LayerMask PickableLayerMask;
    public LayerMask UsableObjects;

    private Item ItemToAdd;
    public GameObject foundDoor;
    private GameObject newItemSlot;

    [Header("Inventory")]
    [SerializeField] private Inventory Inventory;
    [SerializeField] private GameObject InventoryUI;
    [SerializeField] private GameObject ItemSlot;
    [SerializeField] private GridLayoutGroup ItemsContainer;
    
    private void UpdateInventory()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            TryPickupItem();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryOpenDoor();
        }
    }

    public void ToggleInventory()
    {
        InventoryUI.SetActive(!InventoryUI.activeInHierarchy);

        if (InventoryUI.activeInHierarchy)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        } else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void TryOpenDoor()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, SphereRadius, transform.forward, out hit, PickupRadius, UsableObjects))
        {
            if (hit.collider.CompareTag("Door"))
            {
                foundDoor = hit.collider.gameObject;
                newItemSlot.GetComponent<ItemSlotInteract>().door = foundDoor;
                ToggleInventory();
            }
        }
    }

    private void TryPickupItem()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, SphereRadius, transform.forward * SphereOffsetMultiplier, out hit, PickupRadius, PickableLayerMask))
        {
            if (hit.collider.CompareTag("Pickable"))
            {
                ItemToAdd = hit.collider.gameObject.GetComponent<Item>();

                if (ItemsContainer.transform.childCount == 104)
                    return;

                Inventory.Items.Add(ItemToAdd);
                newItemSlot = Instantiate(ItemSlot);
                newItemSlot.transform.SetParent(ItemsContainer.transform, false);
                newItemSlot.GetComponent<RawImage>().texture = ItemToAdd.Sprite;
                newItemSlot.GetComponent<ItemSlotInteract>().item = ItemToAdd;
                hit.collider.gameObject.SetActive(false);

                print($"added {ItemToAdd.Name}");
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + transform.forward * SphereOffsetMultiplier, SphereRadius);
    }

    
}
