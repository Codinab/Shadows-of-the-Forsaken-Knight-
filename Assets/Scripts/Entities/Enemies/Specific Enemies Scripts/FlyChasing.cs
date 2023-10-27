using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class FlyChasing : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    private GameObject _player;
    private float _startPositionX;
    private bool _goingRight;


    //mutual
    private float lastHitTaken;
    private EnemyMovement _enemyMovement;
    [SerializeField] float StunDuration;
    //end of mutual



    //chase speed
    [SerializeField] float HorizontalVelocity = 3;
    //approch (descend) speed and min height
    [SerializeField] float ChaseVelocity = 4;
    //how far is it when it starts descending toward the player
    [SerializeField] float AgressiveRadius = 5;
    //max height and normal speed
    [SerializeField] float VerticalVelocity = 2.5f;
    [SerializeField] float HoverHeight = 5;
    //dont wiggle too much damp
    [SerializeField] float HoverDamp = 0.1f;
    [SerializeField] float RoamRadius = 5;


    // Start is called before the first frame update
    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _startPositionX = transform.position.x;
        _goingRight = true;
        _enemyMovement = GetComponent<EnemyMovement>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (!_enemyMovement.IsCloseToPlayer())
        {
            _rigidbody2D.velocity = Vector2.zero;
            return;
        }
        if (_enemyMovement.pushed)
        {
            lastHitTaken = Time.time;
            _enemyMovement.pushed = false;
        }
        if (lastHitTaken + StunDuration < Time.time)
        {
            //end of mutual
            float playerX = _player.transform.position.x;
            float flyX = transform.position.x;

            if (Vector2.Distance(_player.transform.position, transform.position) < AgressiveRadius)//descend
            {
                ChasePlayer();

            }
            else
            {

                BasicRoam();
                MaintainHeight();
            }
        }
    }
    private void BasicRoam()
    {
        if (_goingRight && DidntPassOnTheRight())
        {
            _rigidbody2D.velocity = new Vector2(HorizontalVelocity, _rigidbody2D.velocity.y);
        }
        else if (!_goingRight && DidntPassOnTheLeft())
        {
            _rigidbody2D.velocity = new Vector2(-HorizontalVelocity, _rigidbody2D.velocity.y);
        }
        else
        {
            _goingRight = !_goingRight;
        }



        //if ()
        //{
        //    _goingRight = true;
        //}
        //else if ()
        //{
        //    _goingRight = false;
        //}
        //Debug.Log($"{transform.position.x + RoamRadius} < {_startPositionX} go left");
        //Debug.Log($"{transform.position.x - RoamRadius} < {_startPositionX} go right");
    }
    private bool DidntPassOnTheRight()
    {
        return transform.position.x - RoamRadius < _startPositionX;
    }
    private bool DidntPassOnTheLeft()
    {
        return transform.position.x + RoamRadius > _startPositionX;
    }
    private void MaintainHeight()
    {

        float vel;
        float height;
        vel = VerticalVelocity;
        height = HoverHeight;


        RaycastHit2D hit = CastRay();

        if (hit.distance < height)//too low, fly up
        {
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, vel);
        }
        else if (hit.distance > height + HoverDamp)//too high, fly down
        {
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, -vel);
        }
        else //perfect stay there
        {
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0);
        }
    }
    private void ChasePlayer()
    {

        Vector2 direction = _player.transform.position - transform.position;
        direction.Normalize();
        _rigidbody2D.velocity = direction * ChaseVelocity;

    }
    private RaycastHit2D CastRay()
    {
        //origin right under the body
        Vector2 origin = new Vector2(transform.position.x, transform.position.y - transform.localScale.y);
        //cast it down
        RaycastHit2D[] rcHits = Physics2D.RaycastAll(origin, Vector2.down);
        foreach (RaycastHit2D hit in rcHits)
        {
            if (hit.collider.tag == "Ground")
            {
                return hit;
            }
        }
        return rcHits[rcHits.Length - 1];
    }
}
