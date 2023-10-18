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
    public Equipment[] EquipmentList
    {
        get
        {
            return _equipment;
        }
    }
     private Equipment[] _equipment;
    private void Start()
    {
        int numberOfSlots = Enum.GetNames(typeof(EquipmentSlot)).Length;
        _equipment = new Equipment[numberOfSlots];
    }

    public void EquipItem(Equipment item)
    {
        int slotIndex = (int)item.equipmentSlot;
        _equipment[slotIndex] = item;
    }
}
