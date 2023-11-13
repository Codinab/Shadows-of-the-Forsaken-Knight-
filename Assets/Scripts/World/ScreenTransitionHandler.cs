using Entities;
using UnityEngine;

namespace World
{
    public class ScreenTransitionHandler : MonoBehaviour
    {
        public string sceneName;
        public Vector2 entrancePosition;

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
                PlayerStats saveData = Player.Instance.SaveStats();
                GameData.SceneTransitionSavedData = new SceneTransitionSavedData(entrancePosition, saveData);
                if (sceneTransitionManager != null)
                {
                    sceneTransitionManager.LoadScene(sceneName);
                }
            }
        }
    }
}