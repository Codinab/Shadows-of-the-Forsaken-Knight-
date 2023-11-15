using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : UI
{
    [SerializeField]
    private Image _heart;
    private Player _player;
    private int _displayedMaxHealth;
    // Start is called before the first frame update
    protected override void Initialize()
    {
    }
    protected override void PlayStarted()
    {
        _player = Player.Instance;
        DisplayMaxHealth();
    }
    protected override void ChildUpdate()
    {
        if( _displayedMaxHealth != _player.MaxHealth )
        {
            DisplayMaxHealth();
        }
        else
        {
            UpdateMissingHealth();
        }
    }



    private void DisplayMaxHealth()
    {
        RemoveDisplayed();
        for (int i = 1; i <= _player.MaxHealth; i++)
        {
            Instantiate(_heart, transform);
            _displayedMaxHealth = i;
        }
        UpdateMissingHealth();
    }
    private void UpdateMissingHealth()
    {
        for(int i = _player.MaxHealth; i > _player.CurrentHealth; i--)
        {
            Image img = transform.GetChild(i-1).gameObject.GetComponent<Image>();
            img.color = Color.black;
        }
        for(int i = 0;i< _player.CurrentHealth; i++)
        {
            Image img = transform.GetChild(i).gameObject.GetComponent<Image>();
            img.color = Color.white;
        }
    }
    private void RemoveDisplayed()
    {
        for (var i = transform.childCount - 1; i >= 0; i--)
        {
            Object.Destroy(transform.GetChild(i).gameObject);
        }
    }
}
