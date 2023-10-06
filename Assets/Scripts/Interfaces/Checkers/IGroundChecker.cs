using UnityEngine;

namespace Interfaces.Checkers
{
    public interface IGroundChecker
    {
        Transform GroundCheck { get; }
        LayerMask WhatIsGround { get; }
        
        public bool TouchingGround { get; set; }
        public bool IsTouchingGround()
        {
            Vector2 boxPosition = GroundCheck.position;
            return Physics2D.OverlapBox(boxPosition, new Vector2(0.9f, 0.1f), 0f, WhatIsGround);
        }
    }
}