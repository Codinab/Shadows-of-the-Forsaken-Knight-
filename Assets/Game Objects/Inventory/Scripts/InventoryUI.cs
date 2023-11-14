using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryUI : UI
{
    [SerializeField] GameObject InventoryTab;
    private Inventory _inventory;
    public Transform ItemsParent;
    InventorySlot[] _slots;
    // Start is called before the first frame update
    
    protected override void Initialize()
    {
        InventoryTab.SetActive(false);
    }
    protected override void PlayStarted()
    {
        _inventory = Inventory.Instance;
        _inventory.onItemChangedCallBack += UpdateInventoryUI;  
        _slots = ItemsParent.GetComponentsInChildren<InventorySlot>();
    }
    protected override void ChildUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            InventoryTab.SetActive(!InventoryTab.activeSelf);
        }
    }


    private void UpdateInventoryUI()
    {
        Item[] items = _inventory.Items;
        for(int i=0;i<_slots.Length;i++)
        {
            if (i < items.Length)
            {
                _slots[i].AddItem(items[i]);
                
            }
            else
            {
                _slots[i].ClearSlot();
            }
        }
    }
}
