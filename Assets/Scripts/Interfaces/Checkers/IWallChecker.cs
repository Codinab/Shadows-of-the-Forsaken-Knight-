﻿using UnityEngine;

namespace Interfaces.Checkers
{
    public interface IWallChecker
    {
        Transform WallCheckLeft { get; set; }
        Transform WallCheckRight { get; set; }
        LayerMask WhatIsGround { get; set; }
        
        public bool TouchingWallLeft { get; set; }
        public bool TouchingWallRight { get; set; }
        public bool TouchingWall { get; set; }

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