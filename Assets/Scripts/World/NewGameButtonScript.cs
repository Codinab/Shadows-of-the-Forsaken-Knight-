using UnityEngine;

public class NewGameButtonScript : MonoBehaviour
{
    
    public void PlayGame()
    {
        var sceneTransitionManager = FindObjectOfType<SceneTransitionManager>();

        var startPosition = new Vector2(-5f, 0);
        GameData.PlayerSaveData = new PlayerSaveData(
            startPosition,
            "Scene 1",
            new PlayerStats()
        );

        sceneTransitionManager.LoadScene("Scene 1");
        
    }
    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}