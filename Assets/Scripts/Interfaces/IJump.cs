using UnityEngine;

namespace Interfaces
{
    public interface IJump : IMovable
    {
        
        protected float JumpForce { get; set; }
        public void RegularJumpRv()
        {
            ResetVerticalVelocity();
            Rigidbody2D.AddForce(new Vector2(0f, JumpForce), ForceMode2D.Impulse);
        }

        public void RegularJump()
        {
            Rigidbody2D.AddForce(new Vector2(0f, JumpForce), ForceMode2D.Impulse);
        }
        
    }
}