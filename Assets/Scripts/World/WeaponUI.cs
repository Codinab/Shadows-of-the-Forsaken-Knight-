using Entities;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class WeaponUI : MonoBehaviour
{
    private CombatHandler _playerCombat;
    private EquipmentManager _equipmentManager;
    private Image _activeImage;
    private TextMeshProUGUI tmp;
    // Start is called before the first frame update
    void Start()
    {
        Player player = Player.Instance;
        _playerCombat = player.CombatHandler;
        _equipmentManager = EquipmentManager.Instance;
        _equipmentManager.onEquipmentChangedCallBack += EquipmentChnaged;
        string display = "";
        foreach (var v in transform.GetChild(0).gameObject.GetComponents<Component>())
        {
            display += v.GetType() + " ";
        }
        Debug.Log(display);
        _activeImage = GetComponent<Image>();
        _activeImage.enabled = false;
        tmp = transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        tmp.text = "";
    }
    private void EquipmentChnaged(Equipment newE, Equipment old)
    {
        if(newE.equipmentSlot != EquipmentSlot.SWORD)
        {
            return;
        }
        else
        {
            _activeImage.sprite = newE.Icon;
            _activeImage.enabled = true;
            tmp.text = _playerCombat.damage.ToString();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
