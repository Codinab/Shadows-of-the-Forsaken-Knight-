using UnityEngine;

public class SmallSpiderChasing : MonoBehaviour
{
    private GameObject _player;
    [SerializeField] int Speed;
    [SerializeField] float ReactionTime;
    [SerializeField] int JumpStrength;
    [SerializeField] GameObject GroundCheckBody;
    Rigidbody2D _rb;
    float lastLookRight;
    float lastLookLeft;
    float lastJump;
    float peakJump;
    bool jumped;
    float lastMoved;
    [SerializeField] LayerMask Ground;
    
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _rb = GetComponent<Rigidbody2D>();
        lastLookLeft = 0;
        lastLookRight = 0;
        lastJump = 0;
        peakJump = transform.position.y;
        jumped = false;
        lastMoved = Time.time;
    }

    void FixedUpdate()
    {
        if (GroundCheck())
        {
            if (_player.transform.position.x > transform.position.x && 
                (lastLookLeft + ReactionTime < Time.time || lastLookLeft == 0))
            {
                _rb.velocity = new Vector2(Speed, _rb.velocity.y);
                lastLookRight = Time.time;
            }
            
            if (_player.transform.position.x < transform.position.x && 
                (lastLookRight + ReactionTime < Time.time || lastLookRight == 0))
            {
                _rb.velocity = new Vector2(-Speed, _rb.velocity.y);
                lastLookLeft = Time.time;
            }
        }

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

        if (_rb.velocity != Vector2.zero)
        {
            lastMoved = Time.time;
        }
        else if (lastMoved + 0.5f < Time.time)
        {
            MoveSlightly();
        }
        
        ClampVelocity();
    }

    private bool GroundCheck()
    {
        return Physics2D.OverlapBox(GroundCheckBody.transform.position, GroundCheckBody.transform.lossyScale, 0f, Ground);
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Ground" && GroundCheck() && lastJump + (float)JumpStrength / 4 < Time.time)
        {
            Jump();
            lastJump = Time.time;
        }
    }
    
    
    
    private void ClampVelocity()
    {
        Vector2 velocity = _rb.velocity;
        if (velocity.x > Speed) velocity.x = Speed;
        if (velocity.x < -Speed) velocity.x = -Speed;
        _rb.velocity = velocity;
    }
    
    private void Jump()
    {
        if (GroundCheck())
        {
            _rb.velocity = new Vector2(_rb.velocity.x, JumpStrength);
            jumped = true;
            peakJump = transform.position.y;
        }
    }
    
    private void MoveSlightly()
    {
        if (_player.transform.position.x > transform.position.x)
        {
            _rb.velocity = new Vector2(1, _rb.velocity.y);
        }
        if (_player.transform.position.x < transform.position.x)
        {
            _rb.velocity = new Vector2(-1, _rb.velocity.y);
        }
    }
}
