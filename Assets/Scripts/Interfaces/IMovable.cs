using UnityEngine;

namespace Interfaces
{
    public interface IMovable
    {
        protected float Speed {get; set;}
        bool MovementDisabledByGetPushed {get; set;}
        Rigidbody2D Rigidbody2D {get; set;}
        public void GetPushed(Vector2 direction, float pushPower, float duration=0.4f)
        {
            MovementDisabledByGetPushed = true;
            ResetVelocities();
            Push(direction, pushPower);
            Invoke(nameof(GetPushedReset), duration);
        }

        protected void Invoke(string functionName, float duration);
        
        void Move(Vector2 direction)
        {
            if (MovementDisabledByGetPushed) return;
            Rigidbody2D.velocity = direction * Speed;
        }
        private void Push(Vector2 direction, float pushPower)
        {
            Rigidbody2D.AddForce(direction * pushPower, ForceMode2D.Impulse);
        }
        private void GetPushedReset()
        {
            MovementDisabledByGetPushed = false;
        }
        
        public void ResetVelocities()
        {
            Rigidbody2D.velocity = new Vector2(0f, 0f);
        }

        public void ResetVerticalVelocity()
        {
            Rigidbody2D.velocity = new Vector2(Rigidbody2D.velocity.x, 0);
        }

        public void ResetHorizontalVelocity()
        {
            Rigidbody2D.velocity = new Vector2(0, Rigidbody2D.velocity.y);

        }
    }
}