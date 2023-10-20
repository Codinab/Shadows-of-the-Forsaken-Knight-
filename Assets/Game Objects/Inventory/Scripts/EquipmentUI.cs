using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentUI : MonoBehaviour
{
    [SerializeField] Transform ItemsParent;
    private EquipmentManager _equipmentManager;
    
    InventorySlot[] _slots;
    // Start is called before the first frame update
    void Start()
    {
        _equipmentManager = EquipmentManager.Instance;
        Inventory.Instance.onItemChangedCallBack += UpdateUI;

        _slots = ItemsParent.GetComponentsInChildren<InventorySlot>();
    }

    private void UpdateUI()
    {
        Item[] items = _equipmentManager.EquipmentList;
        Debug.Log(_slots.Length + " slots in equipment");
        for (int i = 0; i < _equipmentManager.NumberOfSlots; i++)
        {
            if (items[i] == null)
            {
                _slots[i].ClearSlot();
            }
            else
            {
                _slots[i].AddItem(items[i]);
            }
        }
    }
}
