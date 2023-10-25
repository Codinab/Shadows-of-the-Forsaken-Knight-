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
    
    private int _damage = 1;

    public float pushPower = 3f;

    private PlayerHealth _playerHealth;


    private void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        if (_playerMovement == null)
        {
            Debug.LogError("PlayerMovement not found on player");
        }
        _playerHealth = GetComponent<PlayerHealth>();
        if (_playerHealth == null)
        {
            Debug.LogError("PlayerHealth not found on player");
        }
    }

    private void FixedUpdate()
    {
        if (CanAttack() && !_isAttacking)
        {
            StartCoroutine(Attack());
        }
        
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
        if (!attackKey) _attacked = false;
        return (attackKey && !_attacked && _playerHealth.IsAlive());
    }
    
    private bool _isAttacking = false;

    private IEnumerator Attack()
    {
        _isAttacking = true;
        _attacked = true;
    
        // Clear the list of attacked enemies
        attackedEnemies.Clear();
    
        Vector2Int lookingDirection = _playerMovement.GetLookingDirection();
        
        // Execute the attack
        AttackDirection(lookingDirection);
        
        // Push the player back
        if (lookingDirection.y == 0)
        {
            _playerMovement.GetPushed(-lookingDirection, attackPushBack, pushAfterAttackDelay);
        }
    
        // Wait for the attack delay before allowing another attack
        yield return new WaitForSeconds(attackDelay);
    
        // Reset the attack flag
        _isAttacking = false;
    }
    
    public float pushAfterAttackDelay = 0.4f;

    private void AttackDirection(Vector2Int lookingDirection)
    {
        //If the list gets updated during the loop it will crash, so we copy it.
        List<GameObject> objectsInAttackRangeCopy = new List<GameObject>(_objectsInAttackRange);
    
        foreach (GameObject enemy in objectsInAttackRangeCopy)
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
        _playerMovement.RegularJumpRv();
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
