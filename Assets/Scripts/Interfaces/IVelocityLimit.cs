using UnityEngine;

namespace Interfaces
{
    public interface IVelocityLimit
    {
        Rigidbody2D Rigidbody2D { get; }
        float MaxVelocity { get; }
        float MaxFallSpeed { get; }
        bool Dashed { get; set; }
        
        public void ClampPlayerVelocity()
        {
            Vector2 velocity = Rigidbody2D.velocity;
        
            if (velocity.y < -MaxFallSpeed) velocity.y = -MaxFallSpeed;
        
            if (!Dashed)
            {
                if (velocity.x > MaxVelocity) velocity.x = MaxVelocity;
                if (velocity.x < -MaxVelocity) velocity.x = -MaxVelocity;
            }
            Rigidbody2D.velocity = velocity;
        }
    }
}