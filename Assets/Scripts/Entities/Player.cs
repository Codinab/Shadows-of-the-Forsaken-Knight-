using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using Interfaces.Checkers;
using UnityEngine;

namespace Entities
{ 
    public class Player : Character, IVelocityLimit, IGrabbingWallCheck, IDoubleJump, IAttacks
    {
        public float maxFallSpeed;

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnFixedUpdate()
        {
            UpdateActions();

            HandleMovement();

        }

        protected override void PreFixedUpdate()
        {
        }

        protected override void PostFixedUpdate()
        {
            (this as IVelocityLimit).ClampVelocity();
        }

        // Actions
        private void UpdateActions()
        {
            UpdateDirectionKeyPress();

            UpdateTouching();
            UpdateGrabbing();

            UpdateFalling();

            UpdateDoubleJumpCount();
        }

        private void UpdateDoubleJumpCount()
        {
            if (TouchingGround || GrabbingWall) DoubleJumpCount = 0;
        }

        private void UpdateFalling()
        {
            var velocity = Rigidbody2D.velocity;
            Sliding = velocity.y < 0 && GrabbingWall;
            Falling = velocity.y < 0 && !GrabbingWall;

            InAir = !TouchingGround && !GrabbingWall;
        }

        private void UpdateGrabbing()
        {
            GrabbingWallLeft = TouchingWallLeft && Input.GetKey(KeyCode.A);
            GrabbingWallRight = TouchingWallRight && Input.GetKey(KeyCode.D);
            GrabbingWall = GrabbingWallLeft || GrabbingWallRight;
        }

        private void UpdateTouching()
        {
            Vector2 selfSize = transform.localScale;
            Vector2 position = transform.position;

            TouchingGround = Physics2D.OverlapBox(position, new Vector2(selfSize.x - 0.1f, selfSize.y + 10f), 0f);
            TouchingWallLeft = (this as IWallChecker).IsTouchingWallLeft();
            TouchingWallRight = (this as IWallChecker).IsTouchingWallRight();
            TouchingWall = TouchingWallLeft || TouchingWallRight;
        }

        // Movement
        public bool movementEnabled = true;

        public bool MovementEnabled()
        {
            return movementEnabled && !WallJumped;
        }

        private bool CanMoveHorizontally()
        {
            // Only apply regular movement if not in a wall jump state
            return !WallJumped;
        }

        private void HandleMovement()
        {
            if (!movementEnabled) return;

            HandeHorizontalMovement();

            HandleWallGrabbing();

            UpdateJumpKeyPress();
            //if (DashKeyPressed() && CanDash()) Dash();
            if (JumpKeyPressed() && CanJump()) HandleJump();
            //if (JumpKeyPressed() && CanDoubleJump()) DoubleJump();

            //if (AttackKeyPressed() && CanAttack() && !attacked) StartCoroutine(Attack());
        }

        private void HandeHorizontalMovement()
        {
            if (CanMoveHorizontally())
                (this as IMovable).Move(Input.GetAxis("Horizontal"));
        }

        // Looking Direction
        private Vector2Int _lookingDirection = Vector2Int.right;

        public Vector2Int GetLookingDirection()
        {
            return _lookingDirection;
        }

        public bool IsLookingLeft()
        {
            return _lookingDirection.x == -1;
        }

        public bool IsLookingRight()
        {
            return _lookingDirection.x == 1;
        }

        public bool IsLookingUp()
        {
            return _lookingDirection.y == 1;
        }

        public bool IsLookingDown()
        {
            return _lookingDirection.y == -1;
        }


        private Vector2Int _lastHorizontalDirection = Vector2Int.right;
        public int maxSecondaryJumps;

        private void UpdateDirectionKeyPress()
        {
            var rightKey = Input.GetKey(KeyCode.D);
            var leftKey = Input.GetKey(KeyCode.A);
            var upKey = Input.GetKey(KeyCode.W);
            var downKey = Input.GetKey(KeyCode.S);

            if (upKey)
            {
                _lookingDirection = Vector2Int.up;
            }
            else if (downKey)
            {
                _lookingDirection = Vector2Int.down;
            }
            else if (leftKey)
            {
                _lookingDirection = Vector2Int.left;
                _lastHorizontalDirection = Vector2Int.left;
            }
            else if (rightKey)
            {
                _lookingDirection = Vector2Int.right;
                _lastHorizontalDirection = Vector2Int.right;
            }
            else
            {
                // Maintain the last horizontal direction if no vertical keys are pressed
                _lookingDirection = _lastHorizontalDirection;
            }
        }

        // Attack
        public int damage = 1;
        public float attackSpeed = 1f;
        public int pushPower = 3;
        public float attackRange = 1f;
        public int Damage => damage;
        public float AttackSpeed => attackSpeed;
        public int PushPower => pushPower;
        public float AttackRange => attackRange;


        private List<GameObject> objectsInAttackRange = new List<GameObject>();
        private List<GameObject> attackedEnemies = new List<GameObject>();
        private bool isAttacking;
        private bool attacked;

        private void FixedUpdate()
        {
            HandleMovement();
        }
        
        private bool AttackKeyPressed()
        {
            return Input.GetKey(KeyCode.C);
        }

        public bool CanAttack()
        {
            if (AttackKeyPressed() && !attacked)
            {
                attacked = true;
                foreach (var collider in Physics2D.OverlapCircleAll(transform.position, AttackRange))
                    if (collider.gameObject.CompareTag("Enemy") && !objectsInAttackRange.Contains(collider.gameObject))
                        objectsInAttackRange.Add(collider.gameObject);
            }
            else if (!AttackKeyPressed())
            {
                attacked = false;
            }

            return AttackKeyPressed() && !isAttacking;
        }

        public IEnumerator Attack()
        {
            isAttacking = true;

            // Clear the list of attacked enemies
            attackedEnemies.Clear();

            var lookingDirection = GetLookingDirection();

            // Execute the attack
            AttackDirection(lookingDirection);

            // Push the player back
            if (lookingDirection.y == 0) (this as IMovable).GetPushed(-lookingDirection, attackPushBack, pushAfterAttackDelay);

            // Wait for the attack delay before allowing another attack
            yield return new WaitForSeconds(1 / attackSpeed);

            // Reset the attack flag
            isAttacking = false;
        }

        public float pushAfterAttackDelay = 0.4f;
        public float attackPushBack = 0.1f;

        private void AttackDirection(Vector2Int lookingDirection)
        {
            foreach (GameObject enemy in new List<GameObject>(objectsInAttackRange))
            {
                if (attackedEnemies.Contains(enemy)) continue;
                if (EnemyInAttackDirection(lookingDirection, enemy))
                {
                    var enemyLive = enemy.GetComponent<EnemyLive>();
                    var enemyMovement = enemy.GetComponent<EnemyMovement>();

                    enemyLive.TakeDamage(damage);
                    enemyMovement.GetPushed(lookingDirection, pushPower);

                    if (CanJumpAfterSuccessfulDownAttack()) JumpAfterSuccessfulDownAttack();

                    attackedEnemies.Add(enemy);
                }
            }
        }

        private bool CanJumpAfterSuccessfulDownAttack()
        {
            return IsLookingDown();
        }

        private void JumpAfterSuccessfulDownAttack()
        {
            (this as IJump).RegularJumpRv();
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

        // IVelocityLimit
        public float MaxVelocity => 100f;
        public float MaxFallSpeed => maxFallSpeed;
        public bool Dashed { get; set; } // TODO: Implement dash

        // IGrabbingWallCheck
        public bool GrabbingWallLeft { get; set; }
        public bool GrabbingWallRight { get; set; }
        public bool GrabbingWall { get; set; }
        public bool Sliding { get; set; }
        public bool Falling { get; set; }
        public bool InAir { get; set; }

        public int DoubleJumpCount { get; set; }

        public int MaxSecondaryJumps
        {
            get => maxSecondaryJumps;
            set => maxSecondaryJumps = value;
        }

        // Wall Grabbing
        /// <summary>
        ///     Falling speed when grabbing a wall.
        /// </summary>
        public float grabbingFallSpeed = -1f;

        private void HandleWallGrabbing()
        {
            if (!TouchingGround && !WallJumped && Sliding)
            {
                // log in console
                var velocity = Rigidbody2D.velocity;
                Rigidbody2D.velocity = new Vector2(velocity.x, grabbingFallSpeed);
            }
        }

        // Jump
        private bool _jumpKeyPressController;

        private bool CanJump()
        {
            bool result = !_jumpKeyPressController &&
                   !WallJumped &&
                   (TouchingGround || TouchingWallLeft || TouchingWallRight);
            Debug.Log(" !_jumpKeyPressController: " + !_jumpKeyPressController
                + " !WallJumped: " + !WallJumped+
                " TouchingGround: " + TouchingGround);
            if(result) Debug.Log("CanJump()");
            return result;
        }

        private bool JumpKeyPressed()
        {
            var jumpKeyPressed = Input.GetKey(KeyCode.V);
            if (jumpKeyPressed) Debug.Log("Jump() key pressed");
            return jumpKeyPressed;
        }

        private void UpdateJumpKeyPress()
        {
            if (!Input.GetKey(KeyCode.V)) _jumpKeyPressController = false;
        }

        private void HandleJump()
        {
            HandleDirectionOfJump();
            _jumpKeyPressController = true;
        }

        private void HandleDirectionOfJump()
        {
            if (TouchingGround)
            {
                Debug.Log("Jump()");
                (this as IJump).RegularJumpRv();
            }            else if (wallJumpEnabled && TouchingWallRight)
                WallJump(-1);
            else if (wallJumpEnabled && TouchingWallLeft) WallJump(1);
        }


        // IWallJump
        public bool wallJumpEnabled;

        public void WallJump(int direction)
        {
            (this as IMovable).ResetVelocities();
            Rigidbody2D.AddForce(
                new Vector2(direction * wallJumpHorizontalForce, JumpForce * wallJumpVerticalMultiplierForce),
                ForceMode2D.Impulse);
            WallJumped = true;
            Invoke(nameof(ResetWallJump), jumpHorizontalForceDuration);
        }

        private void ResetWallJump()
        {
            WallJumped = false;
            (this as IMovable).ResetVelocities();
        }

        public bool WallJumped { get; set; }
        public float JumpForce { get; set; }

        /// <summary>
        ///     The horizontal force applied when the player jumps off a wall.
        /// </summary>
        public float wallJumpHorizontalForce = 1.25f;

        /// <summary>
        ///     Multiplier for the vertical force applied when the player jumps off a wall.
        /// </summary>
        public float wallJumpVerticalMultiplierForce = 1f;

        /// <summary>
        ///     Duration for which the horizontal force is applied during a wall jump.
        /// </summary>
        public float jumpHorizontalForceDuration = 0.35f;

        // Double Jump
        public bool canDoubleJump;

        private bool CanDoubleJump()
        {
            return canDoubleJump &&
                   !_jumpKeyPressController &&
                   !WallJumped &&
                   InAir &&
                   DoubleJumpCount < MaxSecondaryJumps;
        }

        // Trigger
    }
}