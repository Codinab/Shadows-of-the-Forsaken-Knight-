using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    #region Singleton
    public static EquipmentManager Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("equipment instance duplicated");
        }
        Instance = this;
    }
    #endregion

    public delegate void OnEquipmentChangedCallBack(Equipment newEquipment,Equipment oldEquipment);
    public OnEquipmentChangedCallBack onEquipmentChangedCallBack;
    public Equipment[] EquipmentList
    {
        get
        {
            return _equipment;
        }
    }
    public int NumberOfSlots
    {
        get
        {
            return _numberOfSlots;
        }
    }
    private Equipment[] _equipment;
    private int _numberOfSlots;
    private Inventory _inventory;

    private void Start()
    {
        _numberOfSlots = Enum.GetNames(typeof(EquipmentSlot)).Length;
        _equipment = new Equipment[_numberOfSlots];
        _inventory = Inventory.Instance;
    }

    public void EquipItem(Equipment item)
    {
        Item returnItem=null;
        int slotIndex = (int)item.equipmentSlot;
        if (onEquipmentChangedCallBack != null)
        {
            onEquipmentChangedCallBack.Invoke(item, _equipment[slotIndex]);
        }
        //save item from current equipment
        returnItem = _equipment[slotIndex];
        //cahnge item in equipment
        _equipment[slotIndex] = item;
        //take item from inventory
        _inventory.RemoveItem(item);
        //return old item to inv
        _inventory.AddItem(returnItem);
        //call UI update
        if (_inventory.onItemChangedCallBack != null)
        {
            _inventory.onItemChangedCallBack.Invoke();
        }
    }
}
