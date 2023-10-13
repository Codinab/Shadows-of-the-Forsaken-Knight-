using UnityEngine;

namespace Interfaces
{
    public interface IChaser
    {
        
        protected float CloseDistance { get; }
        protected Camera MainCamera { get; }
        protected Transform Transform { get; }
        protected GameObject Player { get; }
        public bool IsCloseToPlayer()
        {
            float squaredDistance = (Transform.position - Player.transform.position).sqrMagnitude;
            float squaredCloseDistance = CloseDistance * CloseDistance;
            return squaredDistance <= squaredCloseDistance;
        }

        public bool IsInView()
        {
            Vector3 viewportPosition = MainCamera.WorldToViewportPoint(Transform.position);
            return viewportPosition.x >= 0 && viewportPosition.x <= 1 && viewportPosition.y >= 0 && viewportPosition.y <= 1;
        }
    }
}