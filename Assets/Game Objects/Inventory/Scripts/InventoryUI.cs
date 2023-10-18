using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject InventoryTab;
    private Inventory _inventory;
    public Transform ItemsParent;
    InventorySlot[] _slots;
    // Start is called before the first frame update
    void Start()
    {
        _inventory = Inventory.Instance;
        _inventory.onItemChangedCallBack += UpdateInventoryUI;
        _slots = ItemsParent.GetComponentsInChildren<InventorySlot>();
        InventoryTab.SetActive(false);
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            InventoryTab.SetActive(!InventoryTab.activeSelf);
        }
    }
    private void UpdateInventoryUI()
    {
        
        Item[] items = _inventory.Items;
        Debug.Log(_slots.Length);
        for(int i=0;i<_slots.Length;i++)
        {
            Debug.Log($"put in slot {i}");
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
