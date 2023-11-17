using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class UI : MonoBehaviour
{
    private bool _playStarted;
    // Start is called before the first frame update
    private void Start()
    {
        Initialize();
        GameObject menu = GameObject.FindGameObjectWithTag("Menu");
        SceneTransitionManager script = menu.GetComponent<SceneTransitionManager>();
        script.onScreenChanged += StartPlay;
        _playStarted = false;
        foreach(Scene scene in SceneManager.GetAllScenes())
        {
            if(scene.name == "StartMenu")
            {
                _playStarted = false ;
                //PlayStopped();
                return;
            }
        }
        StartPlay();
    }

    //protected abstract void PlayStopped();
    protected abstract void Initialize();
    private void StartPlay()
    {
        _playStarted = true;
        PlayStarted();
    }
    protected abstract void PlayStarted();
    // Update is called once per frame
    void Update()
    {
        if (_playStarted)
        {
            ChildUpdate();
        }
        else
        {
            Debug.Log("play not active");
        }
    }
    protected abstract void ChildUpdate();
}
