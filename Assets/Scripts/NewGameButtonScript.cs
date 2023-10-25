using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameButtonScript : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Scene 1");
    }
}
