using Entities;
using UnityEngine;

namespace World
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class ScreenTransitionHandler : MonoBehaviour
    {
        public string sceneName;  // Name of the scene to transition to
        public Vector3 entrancePosition;  // Position where the player should appear in the new scene

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                // Store the entrance position in a static variable or a singleton
                // so it can be accessed in the new scene
                TransitionData.EntrancePosition = entrancePosition;

                // Load the new scene
                SceneManager.LoadScene(sceneName);
            }
        }
    }

    public static class TransitionData
    {
        public static Vector3 EntrancePosition;
    }
}