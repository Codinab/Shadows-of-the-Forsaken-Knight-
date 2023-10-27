using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using Interfaces.Checkers;
using Unity.VisualScripting;
using UnityEngine;
using World;

namespace Entities
{ 
    public class Player : Character, IVelocityLimit, IGrabbingWallCheck, IDoubleJump, IAttacks
    {
        private EquipmentManager _equipmentManager;
        public float maxFallSpeed;

        public static Player Instance { get; private set; }
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        protected override void Start()
        {
            _combatHandler = GetComponent<CombatHandler>();
            if (_combatHandler == null) Debug.LogError("PlayerCombat not found");
            
            base.Start();
            _equipmentManager = EquipmentManager.Instance;
            _equipmentManager.onEquipmentChangedCallBack += EquipmentChanged; 
            
            // If transitioning from another scene, update the player's position
            if (TransitionData.EntrancePosition != Vector3.zero)
            {
                transform.position = TransitionData.EntrancePosition;
                TransitionData.EntrancePosition = Vector3.zero;  // Reset the entrance position
            }
            
            // Set the z position to -0.5
            transform.position = new Vector3(transform.position.x, transform.position.y, -0.5f);
        }

        protected override void OnFixedUpdate()
        {
            UpdateActions();

            HandleMovement();
            HandleActions();
        }

        protected override void PreFixedUpdate()
        {
        }

        protected override void PostFixedUpdate()
        {
            (this as IVelocityLimit).ClampVelocity();
        }

        #region Actions
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

            (this as IGroundChecker).TouchingGround = (this as IGroundChecker).IsTouchingGround();
                TouchingWallLeft = (this as IWallChecker).IsTouchingWallLeft();
            TouchingWallRight = (this as IWallChecker).IsTouchingWallRight();
            TouchingWall = TouchingWallLeft || TouchingWallRight;
        }
        #endregion

        #region Movement
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
            if (JumpKeyPressed()
               && CanJump()
                ) HandleJump();
            //if (JumpKeyPressed() && CanDoubleJump()) DoubleJump();
        }

        private void HandleActions()
        {
            if (AttackKeyPressed() && CanAttack()) StartCoroutine(Attack());
        }

        private void HandeHorizontalMovement()
        {
            if (CanMoveHorizontally())
                (this as IMovable).Move(Input.GetAxis("Horizontal"));
        }
        #endregion

        #region Looking Direction
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
        #endregion

        #region Attack

        private bool _attackKeyPressed;
        
        private bool AttackKeyPressed()
        {
            var attackKey = Input.GetKey(KeyCode.C);
            return attackKey;
        }

        private CombatHandler _combatHandler;
        public bool CanAttack()
        {
            return (_combatHandler.CanAttack()&&_holdingWeapon);
        }

        public IEnumerator Attack()
        {
            return _combatHandler.Attack();
        }
        #endregion

        #region IVelocityLimit
        public float MaxVelocity => 100f;
        public float MaxFallSpeed => maxFallSpeed;
        public bool Dashed { get; set; } // TODO: Implement dash
        #endregion
        
        #region IGrabbingWallCheck
        public bool GrabbingWallLeft { get; set; }
        public bool GrabbingWallRight { get; set; }
        public bool GrabbingWall { get; set; }

        public float grabbingFallSpeed = -1f;

        private void HandleWallGrabbing()
        {
            if (!(this as IGroundChecker).TouchingGround && !WallJumped && Sliding)
            {
                // log in console
                var velocity = Rigidbody2D.velocity;
                Rigidbody2D.velocity = new Vector2(velocity.x, grabbingFallSpeed);
            }
        }
        #endregion

        public bool Sliding { get; set; }
        public bool Falling { get; set; }
        public bool InAir { get; set; }
        

        // Wall Grabbing
        /// <summary>
        ///     Falling speed when grabbing a wall.
        /// </summary>
        





        #region Jump
        private bool _jumpKeyPressController = false;
        public int maxSecondaryJumps;
        
        private bool CanJump()
        {
            return !_jumpKeyPressController &&
                   !WallJumped &&
                   ((this as IGroundChecker).IsTouchingGround() || TouchingWallLeft || TouchingWallRight);
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
            if ((this as IGroundChecker).IsTouchingGround())
            {
                Debug.Log("Jump()");
                (this as IJump).RegularJumpRv();
            } /*
            else if (wallJumpEnabled && TouchingWallRight) WallJump(-1);
            else if (wallJumpEnabled && TouchingWallLeft) WallJump(1);*/
        }


        #region IWallJump
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
        #endregion
        #region Double Jump
        public int DoubleJumpCount { get; set; }

        public int MaxSecondaryJumps
        {
            get => maxSecondaryJumps;
            set => maxSecondaryJumps = value;
        }
        private bool CanDoubleJump()
        {
            return _canDoubleJump &&
                   !_jumpKeyPressController &&
                   !WallJumped &&
                   InAir &&
                   DoubleJumpCount < MaxSecondaryJumps;
        }
        #endregion

        #endregion

        #region Equipment Related
        private void EquipmentChanged(Equipment newE, Equipment oldE)
        {
            int bonusHealth = 0;
            if (oldE != null)
            {
                bonusHealth -= oldE.HealthModifier;
                _damage -= oldE.DamageModifier;
                switch (oldE.power)
                {
                    case SpecialPower.SWORD:
                        _holdingWeapon = false;
                        break;
                    case SpecialPower.DOUBLE_JUMP:
                        _canDoubleJump = false;
                        break;
                    case SpecialPower.DASH:
                        _canDash = false; 
                        break;
                    case SpecialPower.WALL_JUMP:
                        _canWallJump = false;
                        break;
                    case SpecialPower.VISION:
                        _canSeeInTheDark = false;
                        break;
                    default:
                        break;   
                }
            }
            if (newE != null)
            {
                bonusHealth += newE.HealthModifier;
                _damage += newE.DamageModifier;
                switch (newE.power)
                {
                    case SpecialPower.SWORD:
                        _holdingWeapon = true;
                        break;
                    case SpecialPower.DOUBLE_JUMP:
                        _canDoubleJump = true;
                        break;
                    case SpecialPower.DASH:
                        _canDash = true;
                        break;
                    case SpecialPower.WALL_JUMP:
                        _canWallJump = true;
                        break;
                    case SpecialPower.VISION:
                        _canSeeInTheDark = true;
                        break;
                    default:
                        break;
                }
            }
            MaxHealth += bonusHealth;
            _combatHandler.damage = _damage;
            if (MaxHealth < CurrentHealth)
            {
                CurrentHealth = MaxHealth;
            }
        }
        [SerializeField] 
        private bool _holdingWeapon = false;   //done
        private bool _canDoubleJump = false;   //refactoring needed
        private bool _canDash = false;         //refactoring needed
        private bool _canWallJump = false;     //refactoring needed
        private bool _canSeeInTheDark = false; //not even implemented
        [SerializeField]
        private int _damage = 0;  //done bonus health also done


        #endregion
    }
}