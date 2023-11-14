using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BackgroundParallax : MonoBehaviour
{
    public float xSpeed = 0.0f;
    public float ySpeed = 0.0f;
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
        var position = cameraTransform.position;
        Vector3 cameraDelta = position - lastCameraPosition;
        float parallaxX = cameraDelta.x * xSpeed;
        float parallaxY = cameraDelta.y * ySpeed;
        Vector3 newPosition = imageTransform.position + new Vector3(parallaxX, parallaxY, 0f);
        imageTransform.position = newPosition;
        lastCameraPosition = position;
    }
}
