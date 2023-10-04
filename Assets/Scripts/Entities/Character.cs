using Interfaces;
using UnityEngine.Serialization;

namespace Entities
{
    public abstract class Character : Entity, IJump
    {
        public float jumpForce;
        protected abstract bool CanAttack();
        protected abstract void Attack();


        protected override void OnFixedUpdate()
        {
            if (CanAttack()) Attack();
        }
        
        float IJump.JumpForce { get => jumpForce; set => jumpForce = value; }
    }
}