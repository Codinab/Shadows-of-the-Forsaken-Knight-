using UnityEngine;

namespace Interfaces
{
    public interface IWallJump : IMovable
    {
        public void WallJump(int direction)
        {
            Rigidbody2D.AddForce(new Vector2(direction, JumpForce * WallJumpVerticalMultiplierForce),
                ForceMode2D.Impulse);
            WallJumped = true;
            Invoke(nameof(ResetWallJump), JumpHorizontalForceDuration);
        }
        
        protected bool WallJumped { get; set; }
        protected float JumpForce { get; set; }
        protected float JumpHorizontalForceDuration { get; set; }
        protected float WallJumpVerticalMultiplierForce { get; set; }
        
        private void ResetWallJump()
        {
            WallJumped = false;
            ResetVelocities();
        }
    }
}