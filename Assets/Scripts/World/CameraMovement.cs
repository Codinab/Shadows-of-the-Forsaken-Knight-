using UnityEngine;
using UnityEngine.Serialization;

public class CameraMovement : MonoBehaviour
{
    //private const float MaxHeight = 13.5f;
    
    private GameObject _player;

    private void Start()    
    {
        _player = GameObject.FindWithTag("Player");
        if (_player == null)
        {
            Debug.LogError("Player not found");
        }
    }

    void Update()
    {
        // Follow the player
        if (_player == null) return;
        Vector3 playerPosition = _player.transform.position;
        playerPosition.z -= 10;
        transform.position = playerPosition;
    }
}