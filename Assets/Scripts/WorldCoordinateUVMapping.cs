using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class WorldCoordinateUVMapping : MonoBehaviour
{
    public float textureScale = 1f; // Adjust the scale of the texture

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogError("Renderer not found!");
            return;
        }

        Material material = renderer.material;

        if (material == null)
        {
            Debug.LogError("Material not found!");
            return;
        }

        // Adjust the material's main texture offset based on the object's world position
        Vector2 textureOffset = new Vector2(transform.position.x * textureScale, transform.position.y * textureScale);
        material.mainTextureOffset = textureOffset;
    }
}