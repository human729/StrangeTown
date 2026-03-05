using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CombineItems : MonoBehaviour
{
    [SerializeField] private Inventory Inventory;
    [SerializeField] private GameObject ItemSlot;
    public List<Item> Components;
    public Item Result;
    [SerializeField] private GameObject ItemsContainer;
    
    public void Combine()
    {
        int FoundComponents = 0;
        Item ResultItem = Result;
        List<GameObject> ComponentsToDestroy = new();

        if (!Inventory.Items.Any())
            return;

        foreach (var component in Components)
        {
            foreach (var item in Inventory.Items)
            {
                if (item.GetComponent<ItemSlotInteract>().item.Name == component.Name)
                {
                    ComponentsToDestroy.Add(item);
                    FoundComponents++;
                    break;
                }
            }
        }

        if (FoundComponents == 2)
        {
            foreach (var component in ComponentsToDestroy)
            {
                Destroy(component.gameObject);
                Inventory.Items.Remove(component.gameObject);
            }
            GameObject newItemSlot = Instantiate(ItemSlot);
            newItemSlot.GetComponent<ItemSlotInteract>().item = ResultItem;
            newItemSlot.GetComponent<RawImage>().texture = ResultItem.Sprite;
            newItemSlot.transform.SetParent(ItemsContainer.transform, false);
            Inventory.Items.Add(newItemSlot);
        }
        FoundComponents = 0;
    }
}
