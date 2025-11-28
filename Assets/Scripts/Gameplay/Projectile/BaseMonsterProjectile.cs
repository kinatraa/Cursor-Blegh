using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMonsterProjectile : MonoBehaviour
{
    public float speed = 5f;
    public float existTime = 5f;

    protected Vector3 _targetPosition;
    protected bool isMoving = false;

    public void StartProjectile(Vector3 target)
    {
        _targetPosition = target;
        isMoving = true;
        SetTarget(_targetPosition);
    }
    protected virtual void SetTarget(Vector3 targetPosition = new Vector3())
    {
        StartCoroutine(IEProjectileMove(targetPosition));
    }

    protected virtual IEnumerator IEProjectileMove(Vector3 targetPosition)
    {
        float timer = 0f;

        Vector3 dir = (targetPosition - transform.position).normalized;

        while (timer < existTime && isMoving)
        {
            timer += Time.deltaTime;

            transform.position += dir * (speed * Time.deltaTime);

            yield return null;
        }

        Destroy();
    }


    public void Destroy()
    {
        isMoving = false;
        Destroy(gameObject);
    }
}
