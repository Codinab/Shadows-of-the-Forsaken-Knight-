using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace Interfaces
{
    public interface IMovable
    {
        protected float Speed {get; set;}
        bool MovementDisabledByGetPushed {get; set;}
        Rigidbody2D Rigidbody2D {get; set;}
        public void GetPushed(Vector2 direction, float pushPower)
        {
            MovementDisabledByGetPushed = true;
            ResetVelocities();
            Push(direction, pushPower);
        }
        public void Move(float direction)
        {
            if (MovementDisabledByGetPushed && direction != 0) return;
            Rigidbody2D.velocity = new Vector2(direction * Speed, Rigidbody2D.velocity.y);
        }
        private void Push(Vector2 direction, float pushPower)
        {
            Rigidbody2D.AddForce(direction * pushPower, ForceMode2D.Impulse);
        }
        public void GetPushedReset()
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