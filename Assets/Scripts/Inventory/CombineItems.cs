using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class CombineItems : MonoBehaviour
{
    private int buttonId;
    [SerializeField] private Inventory Inventory;
    [SerializeField] private GameObject ItemSlot;
    public List<Item> Components;
    public Item Result;
    [SerializeField] private GameObject ItemsContainer;
    
    public void Combine()
    {
        int FoundComponents = 0;
        Item ResultItem = Result;

        foreach (var component in Components)
        {
            foreach (var item in Inventory.Items)
            {
                if (item.GetComponent<ItemSlotInteract>().item.Name == component.Name)
                {
                    int itemId = Inventory.Items.IndexOf(item);
                    Destroy(item);
                    print(item.name);
                    Inventory.Items.Remove(item);
                    FoundComponents++;
                    break;
                }
            }
        }

        if (FoundComponents == 2)
        {
            GameObject newItemSlot = Instantiate(ItemSlot);
            newItemSlot.GetComponent<ItemSlotInteract>().item = ResultItem;
            newItemSlot.GetComponent<RawImage>().texture = ResultItem.Sprite;
            newItemSlot.transform.SetParent(ItemsContainer.transform, false);
            Inventory.Items.Add(newItemSlot);
        }
        FoundComponents = 0;
    }
}
