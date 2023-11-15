using Entities;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealingAltar : Interactable

{
    [SerializeField]
    private float _cooldown;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    protected override void Interact()
    {
        HealToFull();
    }
    private void HealToFull()
    {
        Player player = Player.Instance;
        player.CurrentHealth = player.MaxHealth;
        canInteract = false;
        Debug.Log(_cooldown);
        Invoke("CooldownEnd", _cooldown);
    }
    private void CooldownEnd()
    {
        Debug.Log("reset");
        canInteract = true;
    }

}
