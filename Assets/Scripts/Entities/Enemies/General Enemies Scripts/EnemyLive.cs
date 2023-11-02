using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLive : MonoBehaviour
{
    public int health = 1;
    
    public bool IsAlive()
    {
        return health >= 0;
    }
    
    public void TakeDamage(int damage)
    {
        health -= damage;
        DamagedAnimation();
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
    
    public void DamagedAnimation()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        StartCoroutine(ChangeColorTemporarily());
    }
        
    private MeshRenderer meshRenderer;
    private IEnumerator ChangeColorTemporarily()
    {
        if (meshRenderer != null)
        {
            Color originalColor = meshRenderer.material.color;
            var material = meshRenderer.material;
            material.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            material.color = originalColor;
        }
    }
}
