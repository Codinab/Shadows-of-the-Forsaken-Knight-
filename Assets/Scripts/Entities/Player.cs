using Interfaces;
using Interfaces.Checkers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Entities
{
    public class Player : Character, IVelocityLimit, IGrabbingWallCheck, IDoubleJump
    {
        public float maxFallSpeed;

        protected override void OnFixedUpdate()
        {

            base.OnFixedUpdate();
        }

        private void UpdateActions()
        {
            TouchingGround = (this as IGroundChecker).IsTouchingGround();
            TouchingWallLeft = (this as IWallChecker).IsTouchingWallLeft();
            TouchingWallRight = (this as IWallChecker).IsTouchingWallRight();
            TouchingWall = TouchingWallLeft || TouchingWallRight;
            GrabbingWallLeft = TouchingWallLeft && Input.GetKey(KeyCode.A);
            GrabbingWallRight = TouchingWallRight && Input.GetKey(KeyCode.D);
            GrabbingWall = GrabbingWallLeft || GrabbingWallRight;

            // Check if the player is moving downward
            var velocity = Rigidbody2D.velocity;
            Sliding = velocity.y < 0 && GrabbingWall;
            Falling = velocity.y < 0 && !GrabbingWall;

            InAir = !TouchingGround && !GrabbingWall;

            if (TouchingGround || GrabbingWall) DoubleJumpCount = 0;
        }

        private void HandleMovement()
        {
            UpdateDirectionKeyPress();

            HandeHorizontalMovement();
        }

        private void HandeHorizontalMovement()
        {
            /*// Only apply regular movement if not in a wall jump state
            if (!_wallJumped && _movementEnabled)
            {
                _rigidbody2D.velocity = new Vector2(Input.GetAxis("Horizontal") * moveSpeed, _rigidbody2D.velocity.y);
            }*/
        }

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

        /*
         *     private void FixedUpdate()
    {
        HandleMovement();
        UpdateActions();

        HandleWallGrabbing();

        PerformAction();

        ClampPlayerVelocity();
    }
         */

        protected override void PreFixedUpdate()
        {
        }

        protected override void PostFixedUpdate()
        {
        }

        public override bool CanAttack()
        {
            return false;
        }

        public override void Attack()
        {
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
    }
}