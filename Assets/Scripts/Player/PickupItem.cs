using System;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public float PickupRadius;
    public float SphereRadius;
    public LayerMask PickableLayerMask;
    public LayerMask UsableObjects;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            TryPickupItem();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryOpenDoor();
        }
    }

    private void TryOpenDoor()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, SphereRadius, transform.forward, out hit, PickupRadius, UsableObjects))
        {
            if (hit.collider.CompareTag("Door"))
            {
                Door foundDoor = hit.collider.gameObject.GetComponent<Door>();
                

            }
        }
    }

    private void TryPickupItem()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, SphereRadius, transform.forward, out hit, PickupRadius, PickableLayerMask))
        {
            if (hit.collider.CompareTag("Pickable"))
            {
                Item foundItem = hit.collider.gameObject.GetComponent<Item>();
                print($"found {foundItem.Name}");
            }
        }
    }
}
