using Entities;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class WeaponUI : UI
{
    private CombatHandler _playerCombat;
    private EquipmentManager _equipmentManager;
    private Image _activeImage;
    private TextMeshProUGUI tmp;
    // Start is called before the first frame update
    protected override void Initialize()
    {
        
        
        _equipmentManager = EquipmentManager.Instance;
        _equipmentManager.onEquipmentChangedCallBack += EquipmentChnaged;
        _activeImage = GetComponent<Image>();
        _activeImage.enabled = false;
        tmp = transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        tmp.text = "";

    }
    protected override void PlayStarted()
    {
        Player player = Player.Instance;
        _playerCombat = player.CombatHandler;
    }
    protected override void ChildUpdate()
    {
        if(_playerCombat == null)
        {
            Player player = Player.Instance;
            _playerCombat = player.CombatHandler;
        }
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
            string dmg = _playerCombat.damage.ToString();
            tmp.text = dmg; 
        }
    }
    // Update is called once per frame
    
}
