
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    public bool Pushed;
    
    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        Pushed = false;
    }
    
    public void GetPushed(Vector2 direction, float pushPower)
    {
        // Game the script for this object PlayerCombat
        _rigidbody2D.AddForce(direction * pushPower, ForceMode2D.Impulse);
        Pushed = true; //this will tell the chasing code when it was hit and will be set back to false by the chasing code

    }
}
