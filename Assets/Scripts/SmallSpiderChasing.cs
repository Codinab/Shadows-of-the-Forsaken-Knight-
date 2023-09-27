using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
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
    [SerializeField] LayerMask Ground;
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _rb = GetComponent<Rigidbody2D>();
        lastLookLeft = 0;
        lastLookRight = 0;
        lastJump = 0;
        peakJump = transform.position.y;
        jumped = false;
    }

    // Update is called once per frame
    void Update()
    {
        //chasing
        if (GroundCheck())
        {
            if (_player.transform.position.x > transform.position.x)
            {
                if (lastLookLeft + ReactionTime < Time.time || lastLookLeft == 0)
                {
                    _rb.velocity = new Vector2(Speed * Time.deltaTime, _rb.velocity.y);
                    lastLookRight = Time.time;
                }
            }
            if (_player.transform.position.x < transform.position.x)
            {
                if (lastLookRight + ReactionTime < Time.time || lastLookRight == 0)
                {
                    _rb.velocity = new Vector2(-Speed * Time.deltaTime, _rb.velocity.y);
                    lastLookLeft = Time.time;
                }
            }
            //Debug.Log(_rb.velocity);
        }
        if(GroundCheck())
        Debug.Log($"lossy: {GroundCheckBody.transform.lossyScale} local: {GroundCheckBody.transform.localScale}");
        //jumping on trigger
        if (jumped)
        {
            if (peakJump > transform.position.y)
            {
                if (_player.transform.position.x > transform.position.x)
                {
                    _rb.velocity += new Vector2(1, 0);
                }
                if (_player.transform.position.x < transform.position.x)
                {
                    _rb.velocity += new Vector2(-1, 0);
                }
                peakJump = transform.position.y;
                jumped = false;
                Debug.Log("Jumped");
            }
            if (peakJump < transform.position.y)
            {
                peakJump = transform.position.y;
            }
            //Debug.Log(_rb.velocity);
        }
        
    }
    private bool GroundCheck()
    {
        //return _rb.velocity.y == 0;
        return Physics2D.OverlapBox(GroundCheckBody.transform.position, GroundCheckBody.transform.lossyScale, 0f,Ground);
        //return Physics2D.OverlapBox(groundCheck.position, _boxSize, 0f, whatIsGround);
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        
        if (other.tag == "Ground" && GroundCheck() && lastJump + JumpStrength/3 < Time.time)
        {
            Jump();
            lastJump = Time.time;
        }
    }
    
    private void Jump()
    {
        if (GroundCheck())
        {
            _rb.velocity = new Vector2(_rb.velocity.x, JumpStrength);
            jumped = true;
        }
    }
}
