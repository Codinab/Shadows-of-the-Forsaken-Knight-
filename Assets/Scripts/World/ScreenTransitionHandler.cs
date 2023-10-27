using Entities;
using UnityEngine;

namespace World
{
    public class ScreenTransitionHandler : MonoBehaviour
    {
        public string sceneName;
        public Vector3 entrancePosition;

        private SceneTransitionManager sceneTransitionManager;

        private void Start()
        {
            sceneTransitionManager = FindObjectOfType<SceneTransitionManager>();
            if (sceneTransitionManager == null)
            {
                Debug.LogError("SceneTransitionManager not found.");
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                TransitionData.EntrancePosition = entrancePosition;
                if (sceneTransitionManager != null)
                {
                    sceneTransitionManager.LoadScene(sceneName, entrancePosition);
                }
            }
        }
    }

    public static class TransitionData
    {
        public static Vector3 EntrancePosition;
    }
}