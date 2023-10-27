using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameButtonScript : MonoBehaviour
{
    
    public void PlayGame()
    {
        SceneTransitionManager sceneTransitionManager = FindObjectOfType<SceneTransitionManager>();
        sceneTransitionManager.LoadScene("Scene 1", Vector3.zero);
        Debug.Log("invoked");
        
    }
}
