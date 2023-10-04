using UnityEngine;

namespace Interfaces.Checkers
{
    public interface IWallChecker
    {
        Transform WallCheckLeft { get; }
        Transform WallCheckRight { get; }
        LayerMask WhatIsGround { get; }  // Assuming walls and ground use the same layer mask

        public bool IsTouchingWallLeft()
        {
            Vector2 boxPosition = WallCheckLeft.position;
            return Physics2D.OverlapBox(boxPosition, new Vector2(0.1f, 0.9f), 0f, WhatIsGround);
        }

        public bool IsTouchingWallRight()
        {
            Vector2 boxPosition = WallCheckRight.position;
            return Physics2D.OverlapBox(boxPosition, new Vector2(0.1f, 0.9f), 0f, WhatIsGround);
        }
    }


}