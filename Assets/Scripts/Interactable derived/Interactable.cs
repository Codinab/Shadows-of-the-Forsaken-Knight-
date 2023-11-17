using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] float interactDistance = 1.5f;

    protected PlayerInteract playerScript;
    private GameObject _player;
    protected bool canInteract;

    void Start()
    {
        canInteract = true;
        _player = GameObject.FindGameObjectWithTag("Player");
        playerScript = _player.GetComponent<PlayerInteract>();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactDistance);

    }
    private void SetAsPlayerInteractable()
    {
        if (playerScript.onInteractKeyPressedCallBack == null)
        {
            playerScript.onInteractKeyPressedCallBack += PreInteract;
        }
    }
    private void RemoveAsPlayerInteractable()
    {
        playerScript.onInteractKeyPressedCallBack -= PreInteract;
    }
    private void PreInteract()
    {
        playerScript.onInteractKeyPressedCallBack -= PreInteract;
        Interact();
    }
    protected abstract void Interact();
    // Update is called once per frame
    void Update()
    {
        if(_player == null)
        {
            Start();
        }
        float distance = Vector2.Distance(_player.transform.position, transform.position);
        if (distance < interactDistance && canInteract)
        {
            SetAsPlayerInteractable();
        }
        else
        {
            RemoveAsPlayerInteractable();
        }
    }
}
