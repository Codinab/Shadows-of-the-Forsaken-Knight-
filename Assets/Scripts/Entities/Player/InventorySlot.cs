using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image Icon;
    private Item _item;
    

    public void AddItem(Item item)
    {
        Debug.Log("item actually addedf");
        if(item != null)
        {
            Debug.Log("and is not null");
            _item=item;
            Icon.sprite = item.Icon;
            Icon.enabled = true;
        }
    }
    public void ClearSlot()
    {
        Icon.sprite = null;
        Icon.enabled = false;
        _item = null;
    }
    public void UseItem()
    {
        Debug.Log("usnig this");
        if (_item != null)
        {
            _item.Use();
        }
    }
}
