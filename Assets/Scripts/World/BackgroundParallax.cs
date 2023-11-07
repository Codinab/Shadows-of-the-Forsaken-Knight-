using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundParallax : MonoBehaviour
{
    public float parallaxSpeed = 1.0f;
    private Vector3 lastCameraPosition;
    private Transform cameraTransform;
    private Transform imageTransform;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
        imageTransform = transform;
    }

    private void Update()
    {
        Vector3 cameraDelta = cameraTransform.position - lastCameraPosition;
        float parallaxX = cameraDelta.x * parallaxSpeed;
        Vector3 newPosition = imageTransform.position + new Vector3(parallaxX, 0f, 0f);
        imageTransform.position = newPosition;
        lastCameraPosition = cameraTransform.position;
    }
}
