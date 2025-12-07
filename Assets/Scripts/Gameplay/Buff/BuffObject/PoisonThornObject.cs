using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonThornObject : MonoBehaviour
{
    private float _speed = 10f;
    private float _lifetime = 3f;
    private int _damage = 0;

    private Vector2 _direction;
    private Rigidbody2D _rb;
    private bool _hasHit = false;

    void Awake(){
        _rb = GetComponent<Rigidbody2D>();
        if (_rb == null)
        {
            _rb = gameObject.AddComponent<Rigidbody2D>();
        }
        _rb.gravityScale = 0f;
    }

    public void Initialize(Vector2 direction, int damage)
    {
        _direction = direction.normalized;
        _damage = damage;
        
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 90f);

        if (_rb != null)
        {
            _rb.velocity = _direction * _speed;
        }

        StartCoroutine(IEAutoDestroy());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasHit) return;

        if (other.CompareTag(ConstTag.MONSTER))
        {
            BaseMonster monster = other.GetComponent<BaseMonster>();
            if (monster != null && !monster.isDead)
            {
                monster.TakeDamage(_damage);
                _hasHit = true;
                Destroy(gameObject);
            }
        }
    }

    private IEnumerator IEAutoDestroy()
    {
        yield return new WaitForSeconds(_lifetime);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}