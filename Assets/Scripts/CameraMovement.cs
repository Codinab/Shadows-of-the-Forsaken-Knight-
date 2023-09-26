using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    //private const float MaxHeight = 13.5f;
    
    public GameObject player;

    private void Start()    
    {
        if (player == null)
        {
            Debug.LogError("Player not found");
        }
    }

    void Update()
    {
        // Follow the player
        if (player == null) return;
        transform.position = player.transform.position;
    }
}