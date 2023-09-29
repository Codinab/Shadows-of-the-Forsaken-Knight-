
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    
    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }
    
    public void GetPushed(Vector2 direction, float pushPower)
    {
        // Game the script for this object PlayerCombat
        _rigidbody2D.AddForce(direction * pushPower, ForceMode2D.Impulse);
    }
}
