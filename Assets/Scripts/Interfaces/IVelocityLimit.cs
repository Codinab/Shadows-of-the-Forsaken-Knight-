using UnityEngine;

namespace Interfaces
{
    public interface IVelocityLimit
    {
        Rigidbody2D Rigidbody2D { get; }
        float MaxVelocity { get; }
        float MaxFallSpeed { get; }
        bool Dashed { get; set; }
        
        public void ClampVelocity()
        {
            Vector2 velocity = Rigidbody2D.velocity;
        
            if (velocity.y < -MaxFallSpeed) velocity.y = -MaxFallSpeed;
            
            // when falling change the gravity to 2, else to 1 to remove the floating feel
            if (velocity.y < 0) Rigidbody2D.gravityScale = 2;
            else Rigidbody2D.gravityScale = 1;
        
            if (!Dashed)
            {
                if (velocity.x > MaxVelocity) velocity.x = MaxVelocity;
                if (velocity.x < -MaxVelocity) velocity.x = -MaxVelocity;
            }
            Rigidbody2D.velocity = velocity;
        }
    }
}