using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item Item;

    void Start()
    {
        Item.amount = 0;
    }
    void Pickup()
    {
        bool wasPickedUp =  InventoryManager.Instance.Add(Item);
        if (wasPickedUp) Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("I collided with something.");
        Pickup();
    }
}
