using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region Singleton
    public static Inventory Instance;
    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogWarning("More than 1 instance of inventory.");
            return;
        }
        Instance = this;
    }
    #endregion
    [SerializeField] int InventorySize;
    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallBack;
    public Item[] Items { get { return _items.ToArray();  } }
    private List<Item> _items;
    // Start is called before the first frame update
    void Start()
    {
        _items = new List<Item>();
    }

    public bool AddItem(Item item)
    {
        if(item == null)
        {
            return false;
        }
        if(_items.Count >= InventorySize)
        {
            return false;
        }
        _items.Add(item);
        if (onItemChangedCallBack != null)
        {
            onItemChangedCallBack.Invoke();
        }
        return true;
    }
    public void RemoveItem(Item item)
    {
        //string displaymsg = "";
        //foreach (Item i in Items)
        //{
        //    displaymsg += " " + i.name;
        //}
        //Debug.Log("Item removed");
        //Debug.Log(displaymsg);
        _items.Remove(item);
        //displaymsg = string.Empty;
        //foreach (Item i in Items)
        //{
        //    displaymsg += " " + i.name;
        //}
        //Debug.Log(displaymsg);
    }

    //public Item[] SaveInventory()
    //{
    //    return _items.ToArray();
    //}
    //public void LoadInventory(Item[] items)
    //{
    //    foreach (var item in items)
    //    {
    //        AddItem(item);
    //    }
    //}

    //// TODO: temporary fix for the prototype
    //public void Clear()
    //{
    //    _items.Clear();
    //}
}
