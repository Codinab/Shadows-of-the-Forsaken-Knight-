using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static PlayerMovement;


public class PlayerCombat : MonoBehaviour
{
    
    public int health = 5;
    private int _maxHealth = 5;
    private int _damage = 1;
    private float _invincibilityDurationAfterDamaged = 3f;
    private bool _alive = true;
    private bool _invincible = false;
    public float pushPower = 3f;

    

    public bool IsInvincible()
    {
        return _invincible;
    }
    
    public bool Alive
    {
        get => _alive;
        private set => _alive = value;
    }

    private void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        if (_playerMovement == null)
        {
            Debug.LogError("PlayerMovement not found on player");
        }
    }

    private void FixedUpdate()
    {
        UpdateState();
        
        if (CanAttack() && !_isAttacking)
        {
            StartCoroutine(PerformAttack());
        }
        
    }
    
    private void UpdateState()
    {
        _alive = health > 0;
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
    
    private List<GameObject> _objectsInAttackRange = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Enemy")) return;
        if (_objectsInAttackRange.Contains(other.gameObject)) return;
        _objectsInAttackRange.Add(other.gameObject);
        Debug.Log(_objectsInAttackRange.Count + " "+ other.gameObject.name + " entered player trigger");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Enemy")) return;
        _objectsInAttackRange.Remove(other.gameObject);
        //Debug.Log(other.gameObject.name + " exited player trigger");
    }

    private bool CanAttack()
    {
        bool attackKey = Input.GetKey(KeyCode.C);
        return (attackKey && !_attacked);
    }
    
    private bool _isAttacking = false;

    private IEnumerator PerformAttack()
    {
        _isAttacking = true;
    
        // Clear the list of attacked enemies
        attackedEnemies.Clear();
    
        // Execute the attack
        Attack();
    
        // Push the player back
        Vector2 pushBackDirection = -_playerMovement.GetLookingDirection();
        _playerMovement.GetPushed(pushBackDirection, attackPushBack);
    
        // Wait for the attack delay before allowing another attack
        yield return new WaitForSeconds(attackDelay);
    
        // Reset the attack flag
        _isAttacking = false;
    }

    private void Attack()
    {
        Vector2Int lookingDirection = _playerMovement.GetLookingDirection();
        foreach (GameObject enemy in _objectsInAttackRange)
        {
            if (attackedEnemies.Contains(enemy)) continue;
        
            if (enemy.CompareTag("Enemy") && EnemyInAttackDirection(lookingDirection, enemy))
            {
                EnemyLive enemyLive = enemy.GetComponent<EnemyLive>();
                EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
            
                enemyLive.TakeDamage(_damage);
                enemyMovement.GetPushed(lookingDirection, pushPower);
            
                if (CanJumpAfterSuccessfulDownAttack()) JumpAfterSuccessfulDownAttack();
            
                attackedEnemies.Add(enemy);
            }
        }
    }


 
    private bool _attacked = false;
    List<GameObject> attackedEnemies = new List<GameObject>();
    public float attackDelay = 0.25f;
    public float attackPushBack = 0.1f;
    

    
    private bool CanJumpAfterSuccessfulDownAttack()
    {
        return _playerMovement.IsLookingDown();
    }
    
    private void JumpAfterSuccessfulDownAttack()
    {
        _playerMovement.RegularJump();
    }
 
    private bool EnemyInAttackDirection(Vector2Int lookingDirection, GameObject enemy)
    {
        Vector2 enemyPosition = enemy.transform.position;
        Vector2 playerPosition = transform.position;
        Vector2 attackDirection = enemyPosition - playerPosition;

        Vector2 approximatedDirection;

        if (Mathf.Abs(attackDirection.x) > Mathf.Abs(attackDirection.y))
        {
            // x-axis is dominant
            approximatedDirection = new Vector2(Mathf.Sign(attackDirection.x), 0);
        }
        else
        {
            // y-axis is dominant
            approximatedDirection = new Vector2(0, Mathf.Sign(attackDirection.y));
        }

        return approximatedDirection == (Vector2)lookingDirection;
    }
    
    private PlayerMovement _playerMovement;
}
