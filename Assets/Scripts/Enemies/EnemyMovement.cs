
using UnityEngine;
using UnityEngine.Serialization;


[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    public bool Pushed;
    private GameObject _player;
    
    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        Pushed = false;
        
        // Search for the player
        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player == null)
        {
            Debug.LogError("Player not found");
        }

        // Search for the main camera
        _mainCamera = Camera.main;
        if (_mainCamera == null)
        {
            Debug.LogError("Main camera not found");
        }
    }
    
    public void GetPushed(Vector2 direction, float pushPower)
    {
        // Game the script for this object PlayerCombat
        _rigidbody2D.AddForce(direction * pushPower, ForceMode2D.Impulse);
        Pushed = true; //this will tell the chasing code when it was hit and will be set back to false by the chasing code

    }
    
    public float closeDistance = 5.0f;
    private Camera _mainCamera;

    public bool IsCloseToPlayer()
    {
        float squaredDistance = (transform.position - _player.transform.position).sqrMagnitude;
        float squaredCloseDistance = closeDistance * closeDistance;
        return squaredDistance <= squaredCloseDistance;
    }

    public bool IsInView()
    {
        Vector3 viewportPosition = _mainCamera.WorldToViewportPoint(transform.position);
        return viewportPosition.x >= 0 && viewportPosition.x <= 1 && viewportPosition.y >= 0 && viewportPosition.y <= 1;
    }
}
