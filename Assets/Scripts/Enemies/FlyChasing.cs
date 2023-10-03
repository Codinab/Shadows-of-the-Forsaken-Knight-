using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FlyChasing : MonoBehaviour
{
    private Rigidbody2D _rb;
    private GameObject _player;
    //used for reaction time
    private float lastLookLeft;
    private float lastLookRight;

    //chase speed
    [SerializeField] float HorizontalVelocity;
    //approch (descend) speed and min height
    [SerializeField] float CloseHoverHeight;
    [SerializeField] float CloseVerticalVelocity;
    //how far is it when it starts descending toward the player
    [SerializeField] float DescendDistance;
    //max height and normal speed
    [SerializeField] float NormalVerticalVelocity;
    [SerializeField] float NormalHoverHeight;
    //dont wiggle too much damp
    [SerializeField] float HoverDamp=0.01f;
    //time needed to turn around
    [SerializeField] float ReactionTime;


    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _player = GameObject.FindGameObjectWithTag("Player");
        lastLookLeft = 0;
        lastLookRight = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float playerX = _player.transform.position.x;
        float flyX = transform.position.x;
        BasicMove(playerX,flyX);

        
        if (math.abs(playerX - flyX) < DescendDistance)//descend
        {
            MaintainHeight(true);
        }
        else
        {
            MaintainHeight(false);
        }
    }
    private void BasicMove(float playerXPosition,float flyXPosition) 
    {
        if (playerXPosition > flyXPosition &&
                (lastLookLeft + ReactionTime < Time.time || lastLookLeft == 0))
        {
            _rb.velocity = new Vector2(HorizontalVelocity, _rb.velocity.y);
            lastLookRight = Time.time;
        }

        if (playerXPosition < flyXPosition &&
            (lastLookRight + ReactionTime < Time.time || lastLookRight == 0))
        {
            _rb.velocity = new Vector2(-HorizontalVelocity, _rb.velocity.y);
            lastLookLeft = Time.time;
        }
    }
    private void MaintainHeight(bool close)
    {

        float vel;
        float height;
        if (close)
        {
            vel = CloseVerticalVelocity;
            height = CloseHoverHeight;
        }
        else
        {
            vel = NormalVerticalVelocity;
            height = NormalHoverHeight;
        }
        
        //origin right under the body
        Vector2 origin = new Vector2(transform.position.x, transform.position.y - transform.localScale.y);
        //cast it down
        RaycastHit2D rcHit = Physics2D.Raycast(origin, Vector2.down);
        Debug.Log(rcHit.collider.name);
        if(rcHit.collider.tag == "Player" || rcHit.collider.tag == "Collider") // player there just go on him
        {
            _rb.velocity = new Vector2(0, -vel);
        }
        else if (rcHit.distance < height)//too low, fly up
        {
            _rb.velocity = new Vector2(_rb.velocity.x,vel);
        }
        else if(rcHit.distance > height + HoverDamp)//too high, fly down
        {
            _rb.velocity = new Vector2(_rb.velocity.x, -vel);
        }
        else //perfect stay there
        { 
            _rb.velocity = new Vector2(_rb.velocity.x, 0);
        }
    }
}
