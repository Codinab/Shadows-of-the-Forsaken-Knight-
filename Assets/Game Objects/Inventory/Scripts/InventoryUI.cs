using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject InventoryTab;
    private Inventory _inventory;
    private bool _started;
    public Transform ItemsParent;
    InventorySlot[] _slots;
    // Start is called before the first frame update
    void Start()
    {
        InventoryTab.SetActive(false);
        GameObject menu = GameObject.FindGameObjectWithTag("Menu");
        SceneTransitionManager script = menu.GetComponent<SceneTransitionManager>();
        script.onScreenChanged += PlayStarted;
        _started = false;
        if (SceneManager.GetActiveScene().name == "Misho" || SceneManager.GetActiveScene().name == "SampleScene")
        {
            PlayStarted();
        }
    }
    private void PlayStarted()
    {
        if (_started) return;
        _inventory = Inventory.Instance;
        _inventory.onItemChangedCallBack += UpdateInventoryUI;  
        _slots = ItemsParent.GetComponentsInChildren<InventorySlot>();
        _started = true;
    }
    private void Update()
    {
        if (!_started) return;
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            InventoryTab.SetActive(!InventoryTab.activeSelf);
        }
    }
    private void UpdateInventoryUI()
    {
        
        Item[] items = _inventory.Items;
        for(int i=0;i<_slots.Length;i++)
        {
            if (i < items.Length)
            {
                _slots[i].AddItem(items[i]);
                
            }
            else
            {
                _slots[i].ClearSlot();
            }
        }
    }
}
