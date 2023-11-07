using System;
using Entities;
using Interfaces;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyPushing : MonoBehaviour
{
    public float pushPower = 10f;
    public float pushDelay = 0.5f;
    public float stunDuration = 0.2f;
    public int damage = 1;
    public Animator Animator;
    private bool _canPush = true;
    private bool _playerInTrigger = false;

    private GameObject _playerGameObject;
    private Player _player;

    public bool CanMove()
    {
        return _canPush;
    }


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
    
    private void PlayerPushReset()
    {
        (_player as IMovable).GetPushedReset();
    }
    private void PushPlayer()
    {
        if (!_canPush || !_playerInTrigger) return;
        AttackAnimation();
        Vector2 pushDirection = this._playerGameObject.transform.position - transform.position;
        (_player as IMovable).GetPushed(pushDirection.normalized, pushPower);
        Invoke(nameof(PlayerPushReset), stunDuration);
        
        (_player as IHealth).TakeDamage(damage);
        _canPush = false;
        Invoke(nameof(EnablePushing), pushDelay);
    }
    private void AttackAnimation()
    {

    }
}