using System;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyPushing : MonoBehaviour
{
    public float pushPower = 10f;
    public float pushDelay = 0.5f;
    
    private bool _canPush = true;
    private bool _playerInTrigger = false;

    private GameObject _player;
    private PlayerMovement _playerMovement;
    private PlayerCombat _playerCombat;

    private void Start()
    {
        _playerMovement = _player.GetComponent<PlayerMovement>();
        if (_playerMovement == null)
        {
            Debug.LogError("PlayerMovement not found on player");
        }
        
        _playerCombat = _player.GetComponent<PlayerCombat>();
        if (_playerCombat == null)
        {
            Debug.LogError("PlayerCombat not found on player");
        }
        
        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player == null)
        {
            Debug.LogError("Player not found");
        }
        
    } 

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Enemy Trigger");
        if (!other.gameObject.CompareTag("Player")) return;
        //Debug.Log(other.gameObject.name + " entered trigger");
        _playerInTrigger = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        //Debug.Log(other.gameObject.name + " exited trigger");
        _playerInTrigger = false;
    }

    private void EnablePushing()
    {
        _canPush = true;
    }

    private void FixedUpdate()
    {
        PushPlayer();
    }

    private void PushPlayer()
    {
        if (!_canPush || !_playerInTrigger) return;
        
        Vector2 pushDirection = this._player.transform.position - transform.position;
        _playerMovement.GetPushedByEnemy(pushDirection.normalized, pushPower);
        _playerCombat.ApplyDamage(1);
        _canPush = false;
        Invoke(nameof(EnablePushing), pushDelay);
    }
}