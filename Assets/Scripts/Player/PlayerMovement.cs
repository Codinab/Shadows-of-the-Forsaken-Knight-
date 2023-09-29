using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Handles the movement mechanics of the player, including walking, jumping, wall jumping, and dashing.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// The speed at which the player moves horizontally.
    /// </summary>
    public float moveSpeed = 6f;

    /// <summary>
    /// The force applied when the player jumps.
    /// </summary>
    public float jumpForce = 8f;

    /// <summary>
    /// The horizontal force applied when the player jumps off a wall.
    /// </summary>
    public float wallJumpHorizontalForce = 2.5f;

    /// <summary>
    /// Multiplier for the vertical force applied when the player jumps off a wall.
    /// </summary>
    public float wallJumpVerticalMultiplierForce = 1f;

    /// <summary>
    /// Duration for which the horizontal force is applied during a wall jump.
    /// </summary>
    public float jumpHorizontalForceDuration = 0.35f;

    /// <summary>
    /// Cooldown time between dashes.
    /// </summary>
    public float dashCooldown = 0.6f;

    /// <summary>
    /// Duration for which the vertical dash lasts.
    /// </summary>
    public float verticalDashDuration = 0.3f;

    /// <summary>
    /// Falling speed when grabbing a wall.
    /// </summary>
    public float grabbingFallSpeed = -1f;

    /// <summary>
    /// Maximum falling speed of the player.
    /// </summary>
    public float maxFallSpeed = 15f;

    /// <summary>
    /// Power of the player's dash.
    /// </summary>
    public float dashPower = 100f;

    /// <summary>
    /// Number of allowable second jumps.
    /// </summary>
    public int nSecondJumps = 1;
    
    public bool doubleJumpEnabled = false;
    public bool dashEnabled = false;
    public bool wallJumpEnabled = false;

    // Reference to the Rigidbody2D component.
    private Rigidbody2D _rigidbody2D;

    // Whether the player is touching the ground.
    private bool _isGrounded;

    // Whether the player is touching a wall on the left.
    private bool _isTouchingWallLeft;

    // Whether the player is touching a wall on the right.
    private bool _isTouchingWallRight;

    // Whether the player is touching any wall.
    private bool _isTouchingWall;

    // Whether the player is grabbing a wall on the left.
    private bool _isGrabbingWallLeft;

    // Whether the player is grabbing a wall on the right.
    private bool _isGrabbingWallRight;

    // Whether the player is grabbing any wall.
    private bool _isGrabbingWall;

    // Whether the player is currently performing a wall jump.
    private bool _wallJumped;

    // Whether the player can slide down a wall.
    private bool _canSlide;

    // Whether the player is currently falling.
    private bool _isFalling;

    // Whether the player is currently airborne.
    private bool _inTheAir;

    private Vector2Int _lookingDirection = new Vector2Int(1, 0);
    
    // Whether the player's movement is enabled or disabled.
    private bool _movementEnabled = true;

    /// <summary>
    /// Reference point for checking if the player is on the ground.
    /// </summary>
    public Transform groundCheck;

    /// <summary>
    /// Reference point for checking if the player is touching a wall on the left.
    /// </summary>
    public Transform wallCheckLeft;

    /// <summary>
    /// Reference point for checking if the player is touching a wall on the right.
    /// </summary>
    public Transform wallCheckRight;

    /// <summary>
    /// Radius for checking ground or wall collisions.
    /// </summary>
    public float checkRadius = 0.1f;

    /// <summary>
    /// Layer mask to determine what is considered ground.
    /// </summary>
    public LayerMask whatIsGround;

    // Start is called before the first frame update
    private void Start()
    {
        if (playerCombat == null)
        {
            Debug.LogError("PlayerCombat not found on player");
        }
        
        _rigidbody2D = GetComponent<Rigidbody2D>();
        if (_rigidbody2D == null)
        {
            Debug.LogError("Rigidbody2D not found on player");
        }
        
    }

    // Maximum velocity of the player
    private float _maxVelocity = 100f;

    // FixedUpdate is called once per physics frame
    private void FixedUpdate()
    {
        HandleMovement();
        UpdateActions();

        HandleWallGrabbing();

        PerformAction();

        ClampPlayerVelocity();
    }

    private void PerformAction()
    {
        if (!_movementEnabled) return;
        if (PerformedDash()) return;
        if (PerformedJump()) return;
    }
    
    private bool PerformedJump()
    {
        if (CanJump())
        {
            PerformJump();
            return true;
        }
        if (CanDoubleJump())
        {
            PerformConsecutiveJump();
            return true;
        }
        return false;
    }

    private bool PerformedDash()
    {
        if (!dashEnabled) return false;
        if (CanDashVertical())
        {
            PerformDashVertical();
            return true;
        }

        if (CanDashHorizontal())
        {
            PerformDashHorizontal();
            return true;
        }
        
        return false;
    }

    private int _consecutiveJumpsMade;

    private bool CanDoubleJump()
    {
        bool jumpKeyPressed = Input.GetKey(KeyCode.V);
        if (doubleJumpEnabled && jumpKeyPressed && !_jumped && !_wallJumped && _inTheAir && _consecutiveJumpsMade < nSecondJumps)
        {
            return true;
        }

        if (!jumpKeyPressed)
        {
            _jumped = false;
        }

        return false;
    }

    private void ClampPlayerVelocity()
    {
        Vector2 velocity = _rigidbody2D.velocity;
        if (velocity.y < -maxFallSpeed) velocity.y = -maxFallSpeed;
        if (velocity.x > _maxVelocity) velocity.x = _maxVelocity;
        if (velocity.x < -_maxVelocity) velocity.x = -_maxVelocity;
        _rigidbody2D.velocity = velocity;
    }

    private bool _dashed;
    private bool _horizontalDashCooldownActive;
    private bool _verticalDashCooldownActive;

    private float _betweenDashDelay = 0.4f;

    private bool CanDashVertical()
    {
        return Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.B) && !_dashed && !_verticalDashCooldownActive && !_wallJumped;
    }

    private bool CanDashHorizontal()
    {
        return (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && Input.GetKey(KeyCode.B) && !_dashed && !_horizontalDashCooldownActive && !_wallJumped;
    }

    private void PerformDashVertical()
    {
        float vDirection = Input.GetKey(KeyCode.S) ? -0.125f : 0;

        // Reset velocities
        ResetVelocities();

        Vector2 dashDirection = new Vector2(0, vDirection);
        dashDirection.y *= dashPower;
        _rigidbody2D.AddForce(dashDirection, ForceMode2D.Impulse);

        _dashed = true;
        _verticalDashCooldownActive = true;
        ResetCooldownsAfterDelayV();
    }

    private void ResetCooldownsAfterDelayV()
    {
        Invoke(nameof(ResetVerticalDashCooldown), dashCooldown);
        Invoke(nameof(ResetVerticalVelocity), verticalDashDuration);
        Invoke(nameof(ResetBetweenDashCooldown), _betweenDashDelay);
    }

    private void PerformDashHorizontal()
    {
        float hDirection = Input.GetKey(KeyCode.A) ? -1 : Input.GetKey(KeyCode.D) ? 1 : 0;

        // Reset velocities
        ResetVelocities();

        Vector2 dashDirection = new Vector2(hDirection, 0);
        dashDirection.x *= dashPower;
        _rigidbody2D.AddForce(dashDirection, ForceMode2D.Impulse);

        _dashed = true;
        _horizontalDashCooldownActive = true;
        ResetCooldownsAfterDelay();
    }

    private void ResetCooldownsAfterDelay()
    {
        Invoke(nameof(ResetHorizontalDashCooldown), dashCooldown);
        Invoke(nameof(ResetBetweenDashCooldown), _betweenDashDelay);
    }

    private void ResetVerticalDashCooldown()
    {
        _verticalDashCooldownActive = false;
    }

    private void ResetHorizontalDashCooldown()
    {
        _horizontalDashCooldownActive = false;
    }

    private void ResetBetweenDashCooldown()
    {
        _dashed = false;
    }
    
    public void GetPushed(Vector2 direction, float pushPower)
    {
        // Game the script for this object PlayerCombat
        _rigidbody2D.AddForce(direction * pushPower, ForceMode2D.Impulse);
    }
    public void GetPushedByEnemy(Vector2 direction, float pushPower)
    {
        // Game the script for this object PlayerCombat
        if (playerCombat.IsInvincible()) return;
        _movementEnabled = false;
        ResetVelocities();
        GetPushed(direction, pushPower);
        Invoke(nameof(EnableMovement), 0.4f);
    }


    private void EnableMovement()
    {
        _movementEnabled = true;
    }
    
    private void HandleMovement()
    {
        UpdateDirectionKeyPress();

        HandeHorizontalMovement();
    }

    private void HandeHorizontalMovement()
    {
        // Only apply regular movement if not in a wall jump state
        if (!_wallJumped && _movementEnabled)
        {
            _rigidbody2D.velocity = new Vector2(Input.GetAxis("Horizontal") * moveSpeed, _rigidbody2D.velocity.y);
        }
    }

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
    

    private Vector2Int _lastDirection = new Vector2Int(0, 0);
    private void UpdateDirectionKeyPress()
    {
        bool rightKey = Input.GetKey(KeyCode.D);
        bool leftKey = Input.GetKey(KeyCode.A);
        bool upKey = Input.GetKey(KeyCode.W);
        bool downKey = Input.GetKey(KeyCode.S);

        if (!rightKey && !leftKey && !upKey && !downKey)
        {
            _lookingDirection.y = 0;
            _lookingDirection.x = _lastDirection.x;
        }
        else if (upKey && downKey)
        {
            _lookingDirection.y = -_lastDirection.y;
            _lookingDirection.x = 0;
            _lastDirection.y = _lookingDirection.y;
        }
        else if (upKey)
        {
            _lookingDirection.y = 1;
            _lastDirection.y = 1;
            _lookingDirection.x = 0;
        }
        else if (downKey)
        {
            _lookingDirection.y = -1;
            _lastDirection.y = -1;
            _lookingDirection.x = 0;
        }
        else if (leftKey && rightKey)
        {
            _lookingDirection.x = -_lastDirection.x;
            _lookingDirection.y = 0;
            _lastDirection.x = _lookingDirection.x;
        }
        else if (leftKey)
        {
            _lastDirection.x = -1;
            _lookingDirection.x = -1;
            _lookingDirection.y = 0;
        }
        else // rightKey
        {
            _lastDirection.x = 1;
            _lookingDirection.x = 1;
            _lookingDirection.y = 0;
        }
    }

    

    // Box for ground check
    private Vector2 _horizontalTouchRectangle = new Vector2(0.1f, 0.9f);
    private Vector2 _verticalTouchRectangle = new Vector2(0.9f, 0.1f);
    
    private bool IsTouchingGround()
    {
        Vector2 boxPosition = groundCheck.position;
        return Physics2D.OverlapBox(boxPosition, _verticalTouchRectangle, 0f, whatIsGround);
    }

    private bool IsTouchingWallLeft()
    {
        Vector2 boxPosition = wallCheckLeft.position;
        return Physics2D.OverlapBox(boxPosition, _horizontalTouchRectangle, 0f, whatIsGround);
    }

    private bool IsTouchingWallRight()
    {
        Vector2 boxPosition = wallCheckRight.position;
        return Physics2D.OverlapBox(boxPosition, _horizontalTouchRectangle, 0f, whatIsGround);
    }

    private bool _jumped;
    
    private bool CanJump()
    {
        bool jumpKeyPressed = Input.GetKey(KeyCode.V);
        if (jumpKeyPressed && !_jumped && !_wallJumped && (_isGrounded || _isTouchingWallLeft || _isTouchingWallRight))
        {
            return true;
        }

        if (!jumpKeyPressed)
        {
            _jumped = false;
        }
        return false;
    }

    private void PerformJump()
    {
        PerformJumpWithDirection();
        _jumped = true;
    }

    private void PerformConsecutiveJump()
    {
        Debug.Log("Performing consecutive jump");
        RegularJump();
        _jumped = true;
        _consecutiveJumpsMade++;
    }

    private void HandleWallGrabbing()
    {
        if (!_isGrounded && !_wallJumped && _canSlide)
        {
            // log in console
            Vector2 velocity = _rigidbody2D.velocity;
            _rigidbody2D.velocity = new Vector2(velocity.x, grabbingFallSpeed);
        }
    }

    private void UpdateActions()
    {
        _isGrounded = IsTouchingGround();
        _isTouchingWallLeft = IsTouchingWallLeft();
        _isTouchingWallRight = IsTouchingWallRight();
        _isTouchingWall = _isTouchingWallLeft || _isTouchingWallRight;
        _isGrabbingWallLeft = _isTouchingWallLeft && Input.GetKey(KeyCode.A);
        _isGrabbingWallRight = _isTouchingWallRight && Input.GetKey(KeyCode.D);
        _isGrabbingWall = _isGrabbingWallLeft || _isGrabbingWallRight;

        // Check if the player is moving downward
        var velocity = _rigidbody2D.velocity;
        _canSlide = velocity.y < 0 && _isGrabbingWall;
        _isFalling = velocity.y < 0 && !_isGrabbingWall;

        _inTheAir = !_isGrounded && !_isGrabbingWall;

        if (_isGrounded || _isGrabbingWall) _consecutiveJumpsMade = 0;
    }

    private void PerformJumpWithDirection()
    {
        if (_isGrounded)
        {
            RegularJump();
        }
        else if (wallJumpEnabled && _isTouchingWallRight)
        {
            WallJump(-wallJumpHorizontalForce);
        }
        else if (wallJumpEnabled && _isTouchingWallLeft)
        {
            WallJump(wallJumpHorizontalForce);
        }
    }

    public void ResetVelocities()
    {
        _rigidbody2D.velocity = new Vector2(0f, 0f);
    }

    public void ResetVerticalVelocity()
    {
        _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0);
    }

    public void ResetHorizontalVelocity()
    {
        _rigidbody2D.velocity = new Vector2(0, _rigidbody2D.velocity.y);

    }
    

    private void WallJump(float horizontalForce)
    {
        _rigidbody2D.AddForce(new Vector2(horizontalForce, jumpForce * wallJumpVerticalMultiplierForce),
            ForceMode2D.Impulse);
        _wallJumped = true;
        Invoke(nameof(ResetWallJump), jumpHorizontalForceDuration);
    }

    public void RegularJump()
    {
        ResetVerticalVelocity();
        _rigidbody2D.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
    }

    private void ResetWallJump()
    {
        _wallJumped = false;
    }
    
    public PlayerCombat playerCombat;
    
    public void ResetJumps()
    {
        _consecutiveJumpsMade = 0;
        _wallJumped = false;
        _jumped = false;
    }
}