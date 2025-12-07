using System.Collections;
using UnityEngine;

public class InfernoFlameObject : MonoBehaviour
{
    [SerializeField] private float _flameDuration = 3f;
    [SerializeField] private int _damagePerSecond = 3;
    [SerializeField] private float _tickRate = 1f;

    private BaseMonster _targetMonster;
    private Coroutine _burnCoroutine;

    public void Initialize(BaseMonster monster)
    {
        _targetMonster = monster;
        _burnCoroutine = StartCoroutine(IEBurnEffect());
    }

    private IEnumerator IEBurnEffect()
    {
        float elapsed = 0f;
        float nextTickTime = _tickRate;
        
        while (elapsed < _flameDuration)
        {
            if (_targetMonster == null || _targetMonster.isDead)
            {
                Destroy(gameObject);
                yield break;
            }
            
            elapsed += Time.deltaTime;
            
            if (elapsed >= nextTickTime)
            {
                _targetMonster.TakeDamage(_damagePerSecond);
                nextTickTime += _tickRate;
            }
            
            yield return null;
        }
        
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (_burnCoroutine != null)
        {
            StopCoroutine(_burnCoroutine);
            _burnCoroutine = null;
        }
    }
}