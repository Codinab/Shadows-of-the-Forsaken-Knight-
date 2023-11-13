using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLive : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] _spriteRenderer;
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
        StartCoroutine(ChangeColorTemporarily());
    }
        
    private IEnumerator ChangeColorTemporarily()
    {
        List<Color> originalColor = new List<Color>();
        List<Material> material = new List<Material>();
        for (int i =0;i< _spriteRenderer.Length;i++)
        {
            originalColor.Add(_spriteRenderer[i].material.color);
            material.Add(_spriteRenderer[i].material);
            material[i].color = Color.red;
        }
        yield return new WaitForSeconds(0.2f);
        for(int i = 0;i<material.Count;i++)
        {
            material[i].color = originalColor[i];
        }
        
    }

}
