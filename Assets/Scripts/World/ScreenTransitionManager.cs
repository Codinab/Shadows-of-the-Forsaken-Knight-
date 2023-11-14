using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SceneTransitionManager : MonoBehaviour
{
    public Image loadingImage;
    public Canvas loadingCanvas;

    public delegate void OnScreenChanged();
    public OnScreenChanged onScreenChanged;


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

    public void LoadScene(string sceneName)
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
        onScreenChanged.Invoke();
        
        AudioManager.Instance.Stop("StartMenuMusic");
        AudioManager.Instance.Stop("BackgroundSounds");
        AudioManager.Instance.Play("BackgroundSounds");
    }
}

public static class GameData
{
    public static SceneTransitionSavedData SceneTransitionSavedData;
    public static PlayerSaveData PlayerSaveData;

    public static void Reset()
    {
        SceneTransitionSavedData = null;
        PlayerSaveData = null;
        Inventory.Instance.Clear();
        EquipmentManager.Instance.Clear();
    }
}

public class SceneTransitionSavedData
{
    public Vector2 NextSceneEntrancePosition;
    public EquipmentSaveData SavedEquipment;

    public SceneTransitionSavedData(Vector2 nextSceneEntrancePosition, EquipmentSaveData savedEquipment)
    {
        NextSceneEntrancePosition = nextSceneEntrancePosition;
        SavedEquipment = savedEquipment;
    }
}

public class PlayerSaveData
{
    public Vector2 SavedPosition;
    public String SavedSceneName;
    public EquipmentSaveData SavedEquipment;

    public PlayerSaveData(Vector2 savedPosition, String savedSceneName, EquipmentSaveData savedEquipment)
    {
        SavedPosition = savedPosition;
        SavedSceneName = savedSceneName;
        SavedEquipment = savedEquipment;
    }
}

public class EquipmentSaveData
{
    public Item[] Inventory;
    public Equipment[] Equipment;
    public EquipmentSaveData(Item[] inventory, Equipment[] equipment)
    {
        this.Inventory = inventory;
        this.Equipment = equipment;
    }

    public EquipmentSaveData()
    {
        Inventory = new Item[0];
        Equipment = new Equipment[0];
    }
}