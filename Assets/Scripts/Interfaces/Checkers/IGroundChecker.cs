using UnityEngine;

namespace Interfaces.Checkers
{
    public interface IGroundChecker
    {
        LayerMask WhatIsGround { get; }
        
        public bool TouchingGround { get; set; }
        public bool IsTouchingGround(GameObject gameObject)
        {
            // Use the player's size to determine if the player is touching the ground.
            Vector2 size = gameObject.transform.localScale;
            return Physics2D.OverlapBox(gameObject.transform.position, new Vector2(size.x - 0.1f, size.y + 1f), 0f, WhatIsGround);
        }
    }
}