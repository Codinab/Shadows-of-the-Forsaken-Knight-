using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Equipment")]
public class Equipment : Item
{
    public EquipmentSlot equipmentSlot;
    public SpecialPower power;
    public int HealthModifier;
    public int DamageModifier;

    public override void Use()
    {
        base.Use();
        EquipmentManager.Instance.EquipItem(this);
        Inventory.Instance.RemoveItem(this);
    }
}
public enum EquipmentSlot
{
    HELMET,
    TORSO,
    LEGS,
    BOOTS,
    SWORD
}
public enum SpecialPower
{
    NONE,
    SWORD,
    DOUBLE_JUMP,
    DASH,
    WALL_JUMP,
    VISION
}