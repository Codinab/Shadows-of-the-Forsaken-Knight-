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
        int slotIndex = (int)item.equipmentSlot;
        onEquipmentChangedCallBack.Invoke(item, _equipment[slotIndex]);
        if (_equipment[slotIndex] != null)
        {
            _inventory.AddItem(_equipment[slotIndex]);
        }
        _equipment[slotIndex] = item;
        _inventory.RemoveItem(item);
        _inventory.onItemChangedCallBack.Invoke();
    }
}
