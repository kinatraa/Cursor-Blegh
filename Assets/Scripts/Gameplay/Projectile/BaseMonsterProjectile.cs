using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class BaseMonsterProjectile : MonoBehaviour
{
    public float speed = 5f;
    public float existTime = 5f;
    
    protected Vector3 _targetPosition;
    protected bool _isMoving = false;

    private Coroutine _moveCoroutine;

    public void StartProjectile(Vector3 target)
    {
        _targetPosition = target;
        _isMoving = true;
        SetTarget(_targetPosition);
    }
    protected virtual void SetTarget(Vector3 targetPosition = new Vector3())
    {
        // StartCoroutine(IEProjectileMove(targetPosition));
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
        }
        _moveCoroutine = StartCoroutine(IEProjectileMove(targetPosition));
    }

    protected virtual IEnumerator IEProjectileMove(Vector3 targetPosition)
    {
        float timer = 0f;

        Vector3 dir = (targetPosition - transform.position).normalized;

        while (timer < existTime && _isMoving)
        {
            timer += Time.deltaTime;

            transform.position += dir * ((speed) * Time.deltaTime);

            yield return null;
        }

        Destroy();
    }


    public void Destroy()
    {
        _isMoving = false;
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;
        }
        Destroy(gameObject);
    }
}