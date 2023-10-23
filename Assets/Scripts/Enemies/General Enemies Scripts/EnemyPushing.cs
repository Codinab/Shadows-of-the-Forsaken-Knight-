using System;
using Entities;
using Interfaces;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyPushing : MonoBehaviour
{
    public float pushPower = 10f;
    public float pushDelay = 0.5f;
    
    private bool _canPush = true;
    private bool _playerInTrigger = false;

    private GameObject _playerGameObject;
    private Player _player;


    private void Start()
    {
        _playerGameObject = GameObject.FindGameObjectWithTag("Player");
        if (_playerGameObject == null)
        {
            Debug.LogError("Player not found");
        }
        _player = _playerGameObject.GetComponent<Player>();
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
        
        Vector2 pushDirection = this._playerGameObject.transform.position - transform.position;
        (_player as IMovable).GetPushed(pushDirection.normalized, pushPower);
        (_player as IHealth).TakeDamage(1);
        _canPush = false;
        Invoke(nameof(EnablePushing), pushDelay);
    }
}