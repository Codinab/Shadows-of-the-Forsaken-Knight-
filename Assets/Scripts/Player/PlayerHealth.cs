using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    
    public int health = 5;
    private int _maxHealth = 5;
    private bool _alive = true;
    private float _invincibilityDurationAfterDamaged = 3f;
    private bool _invincible = false;

    private PlayerCombat _playerCombat;
    private PlayerMovement _playerMovement;

    public bool IsInvincible()
    {
        return _invincible;
    }

    public bool IsAlive()
    {
        return health > 0;
    }
    
    public void ApplyDamage(int damage)
    {
        if (_invincible) return;
        health -= damage;
        SetInvincible();
        Invoke(nameof(ResetInvincibility), _invincibilityDurationAfterDamaged);
    }
    
    private void SetInvincible()
    {
        _playerMovement.ResetJumps();
        _invincible = true;
    }

    private void ResetInvincibility()
    {
        _invincible = false;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        if (_playerMovement == null)
        {
            Debug.LogError("PlayerMovement not found on player");
        }
        _playerCombat = GetComponent<PlayerCombat>();
        if (_playerCombat == null)
        {
            Debug.LogError("PlayerCombat not found on player");
        }
    }
}
