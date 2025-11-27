using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMonsterProjectile : MonoBehaviour
{
    public float speed = 10f;
    public float existTime = 3f;

    protected virtual void SetTarget(Vector3 targetPosition = new Vector3())
    {
        StartCoroutine(IEProjectileMove(targetPosition));
    }

    protected virtual IEnumerator IEProjectileMove(Vector3 targetPosition)
    {
        float timer = 0f;

        Vector3 dir = (targetPosition - transform.position).normalized;

        while (timer < existTime)
        {
            timer += Time.deltaTime;

            transform.position += dir * (speed * Time.deltaTime);

            yield return null;
        }

        Destroy();
    }


    public void Destroy()
    {
        Destroy(gameObject);
    }
}
