using UnityEngine;

namespace Entities
{
    public abstract class Updatable : MonoBehaviour
    {
        private void FixedUpdate()
        {
            PreFixedUpdate();
            if (DoFixedUpdate()) OnFixedUpdate();
            PostFixedUpdate();
        }

        protected abstract void Start();
        
        
        protected abstract void PreFixedUpdate();
        protected abstract void OnFixedUpdate();
        protected abstract void PostFixedUpdate();

        private bool DoFixedUpdate()
        {
            return ConditionsOnFixedUpdate();
        }
        
        /// <summary>
        /// Method to define conditions to execute OnFixedUpdate
        /// </summary>
        /// <returns></returns>
        protected virtual bool ConditionsOnFixedUpdate()
        {
            return true;
        }
    }
}