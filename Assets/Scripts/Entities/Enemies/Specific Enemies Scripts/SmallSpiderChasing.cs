using UnityEngine;

public class SmallSpiderChasing : MonoBehaviour
{
    private GameObject _player;
    [SerializeField] int Speed;
    [SerializeField] float ReactionTime;
    [SerializeField] int JumpStrength;
    
    private GameObject _groundCheckBody;
    private Rigidbody2D _rigidbody2D;
    private float lastLookRight;
    private float lastLookLeft;
    private float lastJump;
    private float peakJump;
    private bool jumped;
    private float lastMoved;
    private LayerMask _ground;


    //mutual
    private float lastHitTaken;
    [SerializeField] float StunDuration;
    [SerializeField] Animator _animator;

    private EnemyMovement _enemyMovement;
    private EnemyPushing _enemyPushing;
    
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player == null)
        {
            Debug.LogError("Player not found");
        }
        
        _groundCheckBody = transform.Find("GroundCheck").gameObject;
        if (_groundCheckBody == null)
        {
            Debug.LogError("GroundCheck not found");
        }
        
        _ground = LayerMask.GetMask("Ground");
        if (_ground == null)
        {
            Debug.LogError("Ground not found");
        }

        _enemyPushing = GetComponent<EnemyPushing>();
        if (_enemyPushing == null)
        {
            Debug.LogError("EnemyPushing script not found");
        }
        
        _rigidbody2D = GetComponent<Rigidbody2D>();
        lastLookLeft = 0;
        lastLookRight = 0;
        lastJump = 0;
        peakJump = transform.position.y;
        jumped = false;
        lastMoved = Time.time;
        lastHitTaken = 0;
        _enemyMovement = GetComponent<EnemyMovement>();


    }

    void FixedUpdate()
    {
        // Don't do anything until triggered by the player
        if (!_enemyMovement.IsCloseToPlayer())
        {
            HandleAnimtion();
            return;
        }
        if (!_enemyPushing.CanMove())
        {
            HandleAnimtion();
            return;
        }
        if (_enemyMovement.pushed)
        {
            lastHitTaken = Time.time;
            _enemyMovement.pushed= false;
        }
        if (lastHitTaken + StunDuration < Time.time)
        {
            //end of mutual
            BasicMove();
            //if jumped but not moving climb over the wall
            if (jumped)
            {
                if (peakJump > transform.position.y)
                {
                    MoveSlightly();
                    peakJump = transform.position.y;
                    jumped = false;
                }
                if (peakJump < transform.position.y)
                {
                    peakJump = transform.position.y;
                }
            }
            //if stuck on edge move slightly toward player
            if (_rigidbody2D.velocity != Vector2.zero)
            {
                lastMoved = Time.time;
            }
            else if (lastMoved + 0.5f < Time.time)
            {
                MoveSlightly();
            }
        }
        HandleAnimtion();
        //ClampVelocity();
    }

    private bool GroundCheck()
    {
        return Physics2D.OverlapBox(_groundCheckBody.transform.position, _groundCheckBody.transform.lossyScale, 0f, _ground);
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Ground" && GroundCheck() && lastJump + (float)JumpStrength / 4 < Time.time)
        {
            Jump();
            lastJump = Time.time;
        }
    }
    
    private float _maxVelocity = 10f;
    private void ClampVelocity()
    {
        Vector2 velocity = _rigidbody2D.velocity;
        if (velocity.x > _maxVelocity) velocity.x = _maxVelocity;
        if (velocity.x < -_maxVelocity) velocity.x = -_maxVelocity;
        _rigidbody2D.velocity = velocity;
    }
    
    private void Jump()
    {
        if (GroundCheck())
        {
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, JumpStrength);
            jumped = true;
            peakJump = transform.position.y;
        }
    }
    
    private void MoveSlightly()
    {
        if (_player.transform.position.x > transform.position.x)
        {
            _rigidbody2D.velocity = new Vector2(1, _rigidbody2D.velocity.y);
        }
        if (_player.transform.position.x < transform.position.x)
        {
            _rigidbody2D.velocity = new Vector2(-1, _rigidbody2D.velocity.y);
        }
    }
    private void BasicMove()
    {
        if (GroundCheck())
        {
            if (_player.transform.position.x > transform.position.x &&
                (lastLookLeft + ReactionTime < Time.time || lastLookLeft == 0))
            {
                _rigidbody2D.velocity = new Vector2(Speed, _rigidbody2D.velocity.y);
                lastLookRight = Time.time;
            }

            if (_player.transform.position.x < transform.position.x &&
                (lastLookRight + ReactionTime < Time.time || lastLookRight == 0))
            {
                _rigidbody2D.velocity = new Vector2(-Speed, _rigidbody2D.velocity.y);
                lastLookLeft = Time.time;
            }
        }
    }

    private void HandleAnimtion()
    {
        if(GroundCheck())
        {
            _animator.speed = 1;
            if(Mathf.Abs(_rigidbody2D.velocity.x) > 0)
            {
                _animator.SetBool("isMoving", true);
            }
            else
            {
                _animator.SetBool("isMoving", false);
            }
        }
        else
        {
            _animator.speed = 0;
        }
    }
}
