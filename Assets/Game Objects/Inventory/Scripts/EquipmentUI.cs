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

    // Update is called once per frame
    void Update()
    {
        
    }
    private void UpdateUI()
    {
        //create slot code call add ande make them index specific
    }
}
