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
    
    private int _health = 5;
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
        
        if (CanAttack()) AttackLookingDirection();
        
    }
    
    public Text healthText; // This field will hold the reference to the HealthText UI element.

    private void Awake()
    {
        if (healthText == null)
            Debug.LogError("HealthText is not assigned in PlayerCombat!");
    }
    private void UpdateState()
    {
        _alive = _health > 0;

        // Update health display
        if (healthText != null)
            healthText.text = "Health: " + _health;
    }
    
    public void ApplyDamage(int damage)
    {
        if (_invincible) return;
        _health -= damage;
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

    private bool _usedAttackKey = false;
    private bool CanAttack()
    {
        bool attackKey = Input.GetKey(KeyCode.C);
        if (attackKey) Debug.Log("Attack key pressed");
        return (attackKey && !_attacked);
    }
 
    private bool _attacked = false;
    List<GameObject> attackedEnemies = new List<GameObject>();
    private void AttackLookingDirection()
    {
        _attacked = true;
        _usedAttackKey = true;
        
        attackedEnemies.Clear();
        
        Debug.Log("Attack " + _objectsInAttackRange.Count + " " + _playerMovement.GetLookingDirection());
        
        Attack();
        
        Vector2 pushBackDirection = _playerMovement.GetLookingDirection();
        pushBackDirection.x = -pushBackDirection.x;
        _playerMovement.ResetHorizontalVelocity();
        _playerMovement.GetPushed(pushBackDirection, attackPushBack);    
        Invoke(nameof(ResetAttack), attackDelay);

        /*
        InvokeRepeating(nameof(AttackLookingDirectionDelayed), 0f, 0.04f);
        Invoke(nameof(StopAttacking), attackDuration);*/
    }
    
    public float attackDelay = 0.25f;
    public float attackDuration = 0.1f;
    public float attackPushBack = 0.1f;
    
    /*private void StopAttacking()
    {
        CancelInvoke(nameof(AttackLookingDirectionDelayed)); // Stop the repeated invoking of Attacking
    }

    private void AttackLookingDirectionDelayed()
    {
        Attack();
    }*/
    
    private bool CanJumpAfterSuccessfulDownAttack()
    {
        return _playerMovement.IsLookingDown();
    }
    
    private void JumpAfterSuccessfulDownAttack()
    {
        _playerMovement.RegularJump();
    }

    private void Attack()
    {
        Vector2Int lookingDirection = _playerMovement.GetLookingDirection();

        List<GameObject> enemiesToAttack = new List<GameObject>();
        enemiesToAttack.AddRange(_objectsInAttackRange);
        
        foreach (GameObject enemy in enemiesToAttack)
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

    private void ResetAttack()
    {
        Debug.Log("Reset attack");
        _attacked = false;
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
