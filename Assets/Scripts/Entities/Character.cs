using Interfaces;
using Interfaces.Checkers;
using UnityEngine;

namespace Entities
{

    public abstract class Character : Entity, IJump, IGroundChecker, IWallChecker
    {
        public float jumpForce;
        public abstract bool CanAttack();
        public abstract void Attack();

        protected override void Start()
        {
            base.Start();
            GroundCheck = transform.Find("GroundCheck");
            if (GroundCheck == null) Debug.LogError("GroundCheck not found");
            
            WhatIsGround = LayerMask.GetMask("Ground");
            if (WhatIsGround == 0) Debug.LogError("LayerMask for ground not found");
            
            WallCheckLeft = transform.Find("WallCheckLeft");
            if (WallCheckLeft == null) Debug.LogError("WallCheckLeft not found");
            
            WallCheckRight = transform.Find("WallCheckRight");
            if (WallCheckRight == null) Debug.LogError("WallCheckRight not found");
        }

        protected override void OnFixedUpdate()
        {
            if (CanAttack()) Attack();
        }

        // IJump
        float IJump.JumpForce { get => jumpForce; set => jumpForce = value; }
        
        // IGroundChecker
        public Transform GroundCheck { get; set; }
        public LayerMask WhatIsGround { get; set; }
        public bool TouchingGround { get; set; }
        
        // IWallChecker
        public Transform WallCheckLeft { get; set; }
        public Transform WallCheckRight { get; set; }
        public bool TouchingWallLeft { get; set; }
        public bool TouchingWallRight { get; set; }
        public bool TouchingWall { get; set; }
        
    }
}