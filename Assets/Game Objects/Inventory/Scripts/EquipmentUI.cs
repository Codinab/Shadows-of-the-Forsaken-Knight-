using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class EquipmentUI : MonoBehaviour
{
    [SerializeField] Transform ItemsParent;
    private EquipmentManager _equipmentManager;
    private bool _started;
    InventorySlot[] _slots;
    // Start is called before the first frame update
    void Start()
    {
        _started = false;
        GameObject menu = GameObject.FindGameObjectWithTag("Menu");
        SceneTransitionManager script = menu.GetComponent<SceneTransitionManager>();
        script.onScreenChanged += PlayStarted;
        if (SceneManager.GetActiveScene().name == "Misho" || SceneManager.GetActiveScene().name == "SampleScene")
        {
            PlayStarted();
        }
    }
    private void PlayStarted()
    {
        if(_started) return;
        _started=true;
        _equipmentManager = EquipmentManager.Instance;
        Debug.Log(_equipmentManager);
        Inventory.Instance.onItemChangedCallBack += UpdateUI;
        _slots = ItemsParent.GetComponentsInChildren<InventorySlot>();

    }
    private void UpdateUI()
    {
        Item[] items = _equipmentManager.EquipmentList;
        for (int i = 0; i < _equipmentManager.NumberOfSlots; i++)
        {
            if (items[i] == null)
            {
                _slots[i].ClearSlot();
            }
            else
            {
                _slots[i].AddItem(items[i]);
            }
        }
    }
}
