using Interfaces;
using UnityEngine;

namespace Entities
{
    public abstract class Entity : Updatable, IMovable, IHealth
    {
        protected Entity(int maxHealth)
        {
            Rigidbody2D = GetComponent<Rigidbody2D>();
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
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; set; }
        public void TakeDamage(int amount)
        {
            throw new System.NotImplementedException();
        }

        public void Heal(int amount)
        {
            throw new System.NotImplementedException();
        }

        public bool IsDead { get; }
    }
}