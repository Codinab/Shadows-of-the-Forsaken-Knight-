using Interfaces;
using UnityEngine;

namespace Entities
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class Entity : Updatable, IMovable, IHealth
    {
        public int maxHealth = 5;
        public int health = 5;
        protected void Start()
        {
            Rigidbody2D = GetComponent<Rigidbody2D>();
            if (Rigidbody2D == null) Debug.LogError("Rigidbody2D not found");
            MovementDisabledByGetPushed = false;
            MaxHealth = maxHealth;
            CurrentHealth = MaxHealth;
        }
        
        
        // IMovable
        public bool MovementDisabledByGetPushed { get; set; }
        public Rigidbody2D Rigidbody2D { get; set; }

        void IMovable.Invoke(string functionName, float duration)
        {
            Invoke(functionName, duration);
        }

        
        // IHealth
        public int CurrentHealth { get => health; set => health = value;}
        public int MaxHealth { get => maxHealth; set => maxHealth = value; }

    }
}