using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollisionHandler : MonoBehaviour
{
    Rigidbody2D _rb;
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag != "Ground")
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider, true);
        }
    }
}
