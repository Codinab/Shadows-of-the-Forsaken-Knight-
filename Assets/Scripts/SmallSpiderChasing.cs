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
    [SerializeField] int JumpStrenght;
    Rigidbody2D _rb;
    float lastLookRight;
    float lastLookLeft;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _rb = GetComponent<Rigidbody2D>();
        lastLookLeft = 0;
        lastLookRight = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (_player == null) return;
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
        }
        
        //jumping in on trigger
        
    }
    private bool GroundCheck()
    {
        return _rb.velocity.y == 0;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Ground")
        {
            Jump();
        }
    }
    private void Jump()
    {
        _rb.velocity = new Vector2(_rb.velocity.x, JumpStrenght);
    }
}
