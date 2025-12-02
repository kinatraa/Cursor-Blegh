using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostPrisonObject : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    public float _frostDuration = 5f;

    void Start()
    {
        StartCoroutine(IEDestroy());
    }

    private IEnumerator IEDestroy()
    {
        yield return new WaitForSeconds(_frostDuration);
        
        Destroy(gameObject);
    }
}
