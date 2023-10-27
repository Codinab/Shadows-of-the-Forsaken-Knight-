using Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

namespace Entities
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class Entity : Updatable, IMovable, IHealth
    {
        [SerializeField]
        private int _maxHealth = 5;
        [SerializeField]
        private int _health;
        public float speed;

        protected override void Start()
        {
            Rigidbody2D = GetComponent<Rigidbody2D>();
            if (Rigidbody2D == null) Debug.LogError("Rigidbody2D not found");
            MovementDisabledByGetPushed = false;
            MaxHealth = _maxHealth;
            CurrentHealth = MaxHealth;

        }
        
        // IMovable
        float IMovable.Speed { get => speed; set => speed = value; }
        public bool MovementDisabledByGetPushed { get; set; }
        public Rigidbody2D Rigidbody2D { get; set; }




        // IHealth
        
        public int CurrentHealth { get => _health; set => _health = value;}
        public virtual int MaxHealth { get => _maxHealth; set => _maxHealth = value; }

    }
}