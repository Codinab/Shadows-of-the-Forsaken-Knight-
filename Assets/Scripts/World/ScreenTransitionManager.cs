using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    public Image loadingImage;
    public Canvas loadingCanvas;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        FindLoadingScreenComponents();
        loadingCanvas.enabled = false;
    }

    private void FindLoadingScreenComponents()
    {
        if (loadingCanvas == null)
        {
            Debug.LogError("No Canvas found in the scene.");
            return;
        }

        if (loadingImage == null) Debug.LogError("No Image found as a child of the Canvas.");
    }

    public void LoadScene(string sceneName, Vector3 entrancePosition)
    {
        SetRandomLoadingImage();
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    public void SetRandomLoadingImage()
    {
        #if UNITY_EDITOR
        
        var path = "Assets/Scenes/Load Screens";
        var files = AssetDatabase.FindAssets("", new[] { path });
        if (files.Length > 0)
        {
            var randomIndex = Random.Range(0, files.Length);
            var assetPath = AssetDatabase.GUIDToAssetPath(files[randomIndex]);
            var randomSprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            if (randomSprite != null)
                loadingImage.sprite = randomSprite;
            else
                Debug.LogWarning("No sprite found at path: " + assetPath);
        }
        else
        {
            Debug.LogWarning("No files found in folder: " + path);
        }
        
        #endif
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // Show the loading screen
        loadingCanvas.enabled = true;

        // Load the scene asynchronously
        var asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        yield return asyncLoad;

        // Hide the loading screen
        loadingCanvas.enabled = false;
    }
}