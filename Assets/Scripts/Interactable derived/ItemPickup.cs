using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : Interactable
{
    
    public Item Item;
    // Start is called before the first frame update
    
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
    protected override void Interact()
    {
        

        if (Inventory.Instance.AddItem(Item))
        {
            AudioManager.Instance.Play("ItemPickUp");

            Destroy(gameObject);
        }
       
    }
    
}
