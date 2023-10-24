using System;
using Interfaces;
using Interfaces.Checkers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Entities
{
    
    public class IntEventArgs : EventArgs
    {
        public IntEventArgs(int value)
        {
            Value = value;
        }
        public int Value { get; set; }
    }
    public class Player : Character, IVelocityLimit, IGrabbingWallCheck, IDoubleJump
    {
        private EquipmentManager _em;
        public float maxFallSpeed;
        
        protected override void Start()
        {
            base.Start();
            _em = EquipmentManager.Instance;
            _em.onEquipmentChangedCallBack += EquipmentChanged; 
        }

        protected override void OnFixedUpdate()
        {
            UpdateActions();
            HandleMovement();
            
            base.OnFixedUpdate();
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
            TouchingGround = (this as IGroundChecker).IsTouchingGround();
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
            
            UpdateJumpKeyPress();
            //if (JumpKeyPressed() && CanJump())(this as IJump).RegularJumpRv();
            if (JumpKeyPressed() && CanJump()) HandleJump();
            //if (JumpKeyPressed() && CanDoubleJump()) DoubleJump();
            
            
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
        #endregion


        #region Attack
        public override bool CanAttack()
        {
            return false;
        }

        public override void Attack()
        {
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
        #endregion
        public bool Sliding { get; set; }
        public bool Falling { get; set; }
        public bool InAir { get; set; }

        #region Jump
        private bool _jumpKeyPressController = false;
        private bool CanJump()
        {
            return (!_jumpKeyPressController && 
                    !WallJumped &&
                    (TouchingGround || TouchingWallLeft || TouchingWallRight));
        }
        
        private bool JumpKeyPressed()
        {
            bool jumpKeyPressed = Input.GetKey(KeyCode.V);
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
                (this as IJump).RegularJumpRv();
            }
            else if (wallJumpEnabled && TouchingWallRight)
            {
                WallJump(-1);
            }
            else if (wallJumpEnabled && TouchingWallLeft)
            {
                WallJump(1);
            }
        }


        #region IWallJump
        public bool wallJumpEnabled = false;
        public void WallJump(int direction)
        {
            (this as IMovable).ResetVelocities();
            Rigidbody2D.AddForce(new Vector2(direction * wallJumpHorizontalForce, JumpForce * wallJumpVerticalMultiplierForce),
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
        /// The horizontal force applied when the player jumps off a wall.
        /// </summary>
        public float wallJumpHorizontalForce = 1.25f;

        /// <summary>
        /// Multiplier for the vertical force applied when the player jumps off a wall.
        /// </summary>
        public float wallJumpVerticalMultiplierForce = 1f;

        /// <summary>
        /// Duration for which the horizontal force is applied during a wall jump.
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
            return (canDoubleJump && 
                    !_jumpKeyPressController && 
                    !WallJumped && 
                    InAir &&
                    DoubleJumpCount < MaxSecondaryJumps);
        }
        #endregion

        #endregion

        #region Equipment Related
        private void EquipmentChanged(Equipment oldE, Equipment newE)
        {
            if (oldE != null)
            {
                bonusHealth -= oldE.HealthModifier;
                damageModifier -= oldE.DamageModifier;
                switch(oldE.power)
                {
                    case SpecialPower.SWORD:
                        holdingWeapon = false;
                        break;
                    case SpecialPower.DOUBLE_JUMP:
                        canDoubleJump = false;
                        break;
                    case SpecialPower.DASH:
                        canDash = false; 
                        break;
                    case SpecialPower.WALL_JUMP:
                        canWallJump = false;
                        break;
                    case SpecialPower.VISION:
                        canSeeInTheDark = false;
                        break;
                    default:
                        break;   
                }
            }
            if (newE != null)
            {
                bonusHealth += oldE.HealthModifier;
                damageModifier += oldE.DamageModifier;
                switch (oldE.power)
                {
                    case SpecialPower.SWORD:
                        holdingWeapon = true;
                        break;
                    case SpecialPower.DOUBLE_JUMP:
                        canDoubleJump = true;
                        break;
                    case SpecialPower.DASH:
                        canDash = true;
                        break;
                    case SpecialPower.WALL_JUMP:
                        canWallJump = true;
                        break;
                    case SpecialPower.VISION:
                        canSeeInTheDark = true;
                        break;
                    default:
                        break;
                }
            }
        }
        private bool holdingWeapon = false;
        private bool canDoubleJump = false;
        private bool canDash = false;
        private bool canWallJump = false;
        private bool canSeeInTheDark = false;
        private int bonusHealth = 0;
        private int damageModifier = 0;

        #endregion
    }
}