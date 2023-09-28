using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLive : MonoBehaviour
{
    public int health = 1;
    
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
