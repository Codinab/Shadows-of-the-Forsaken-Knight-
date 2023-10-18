using System.Collections;
using System.Collections.Generic;
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
        if(_items.Count > InventorySize)
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
        _items.Remove(item);
    }
}
