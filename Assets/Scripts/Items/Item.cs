using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Interfaces;
using UnityEngine.Serialization;
using Entities;

[CreateAssetMenu(fileName ="New Item",menuName ="Inventory/Item")] 
public class Item : ScriptableObject
{
    new public string Name = "New Item";
    public Sprite Icon;
    

    public virtual void Use()
    {
    }
}
