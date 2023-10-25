using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

namespace Entities
{
    public class CombatHandler : MonoBehaviour
    {
        private readonly int _damage = 1;

        public float pushPower = 3f;

        private Player _player;


        private void Start()
        {
            _player = GetComponent<Player>();
            if (_player == null) Debug.LogError("Player not found");
        }

        private readonly List<GameObject> _objectsInAttackRange = new();

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Enemy")) return;
            if (_objectsInAttackRange.Contains(other.gameObject)) return;
            _objectsInAttackRange.Add(other.gameObject);
            Debug.Log(_objectsInAttackRange.Count + " " + other.gameObject.name + " entered player trigger");
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Enemy")) return;
            _objectsInAttackRange.Remove(other.gameObject);
        }

        public bool CanAttack()
        {
            return !(_player as IHealth).IsDead() && 
                   !_isAttacking;
        }

        private bool _isAttacking;
        private void GetPushedReset()
        {
            (_player as IMovable).GetPushedReset();
        }

        public IEnumerator Attack()
        {
            _isAttacking = true;

            // Clear the list of attacked enemies
            _attackedEnemies.Clear();

            var lookingDirection = _player.GetLookingDirection();

            // Execute the attack
            AttackDirection(lookingDirection);

            // Push the player back
            if (lookingDirection.y == 0)
            {
                (_player as IMovable).GetPushed(-lookingDirection, attackPushBack);
                Invoke(nameof(GetPushedReset), pushAfterAttackDelay);
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
            var objectsInAttackRangeCopy = new List<GameObject>(_objectsInAttackRange);

            foreach (var enemy in objectsInAttackRangeCopy)
            {
                if (_attackedEnemies.Contains(enemy)) continue;
                if (enemy.CompareTag("Enemy") && EnemyInAttackDirection(lookingDirection, enemy))
                {
                    var enemyLive = enemy.GetComponent<EnemyLive>();
                    var enemyMovement = enemy.GetComponent<EnemyMovement>();

                    enemyLive.TakeDamage(_damage);
                    enemyMovement.GetPushed(lookingDirection, pushPower);

                    if (CanJumpAfterSuccessfulDownAttack()) JumpAfterSuccessfulDownAttack();

                    _attackedEnemies.Add(enemy);
                }
            }
        }

        private List<GameObject> _attackedEnemies = new();
        public float attackDelay = 0.25f;
        public float attackPushBack = 0.1f;


        private bool CanJumpAfterSuccessfulDownAttack()
        {
            return _player.IsLookingDown();
        }

        private void JumpAfterSuccessfulDownAttack()
        {
            (_player as IJump).RegularJumpRv();
        }

        private bool EnemyInAttackDirection(Vector2Int lookingDirection, GameObject enemy)
        {
            Vector2 enemyPosition = enemy.transform.position;
            Vector2 playerPosition = transform.position;
            var attackDirection = enemyPosition - playerPosition;

            Vector2 approximatedDirection;

            if (Mathf.Abs(attackDirection.x) > Mathf.Abs(attackDirection.y))
                // x-axis is dominant
                approximatedDirection = new Vector2(Mathf.Sign(attackDirection.x), 0);
            else
                // y-axis is dominant
                approximatedDirection = new Vector2(0, Mathf.Sign(attackDirection.y));

            return approximatedDirection == lookingDirection;
        }
    }
}