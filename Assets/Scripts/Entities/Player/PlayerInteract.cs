using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{

    public delegate void OnInteractKeyPressed();
    public OnInteractKeyPressed onInteractKeyPressedCallBack;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E)) 
        {
            if(onInteractKeyPressedCallBack != null)
            {
                onInteractKeyPressedCallBack.Invoke();
            }
        }

    }
}
