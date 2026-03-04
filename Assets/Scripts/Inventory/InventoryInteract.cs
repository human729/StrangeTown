using UnityEngine;

public class InventoryInteract : MonoBehaviour
{
    [SerializeField] private GameObject CombineMenu;
    [SerializeField] private GameObject Inventory;

    public void OpenInventory()
    {
        CombineMenu.SetActive(false);
        Inventory.SetActive(true);
    }

    public void OpenCombineMenu()
    {
        Inventory.SetActive(false);
        CombineMenu.SetActive(true);
    }    
}
