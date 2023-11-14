using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] float interactDistance=1.5f;
    public Item Item;
    private PlayerInteract _playerScript;
    private GameObject _player;
    // Start is called before the first frame update
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
        
    }
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerScript = _player.GetComponent<PlayerInteract>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float distance = Vector2.Distance(_player.transform.position, transform.position);
        if (distance < interactDistance)
        {
            SetAsPlayerInteractable();
        }
        else
        {
            RemoveAsPlayerInteractable();
        }
    }
    private void PickUp()
    {
        

        if (Inventory.Instance.AddItem(Item))
        {
            AudioManager.Instance.Play("ItemPickUp");
            _playerScript.onInteractKeyPressedCallBack -= PickUp;
            Destroy(gameObject);
        }
       
    }
    private void SetAsPlayerInteractable()
    {
        if (_playerScript.onInteractKeyPressedCallBack == null)
        {
            _playerScript.onInteractKeyPressedCallBack += PickUp;
        }
    }
    private void RemoveAsPlayerInteractable()
    {
        _playerScript.onInteractKeyPressedCallBack -= PickUp;
    }
}
