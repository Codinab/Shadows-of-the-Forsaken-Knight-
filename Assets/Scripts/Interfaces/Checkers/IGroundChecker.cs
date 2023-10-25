using UnityEngine;

namespace Interfaces.Checkers
{
    public interface IGroundChecker
    {
        LayerMask WhatIsGround { get; }
        
        Transform GroundCheck { get; set; }
        public bool TouchingGround { get; set; }
        public bool IsTouchingGround()
        {
            Vector2 boxPosition = GroundCheck.position;
            return Physics2D.OverlapBox(boxPosition, new Vector2(0.9f, 0.1f), 0f, WhatIsGround);
        }
    }
}