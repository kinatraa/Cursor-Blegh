using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoltenStoneObject : MonoBehaviour
{
    private TrailRenderer _trailRenderer;
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _damageTickRate = 0.5f; 
    [SerializeField] private float _trailStartWidth = 0.3f;
    [SerializeField] private float _trailEndWidth = 0.1f;
    private Material _trailMaterial;

    private Transform _weaponTransform;
    private float _trailDuration = 1;
    private int _damage;
    private HashSet<BaseMonster> _damagedMonsters = new HashSet<BaseMonster>();
    private float _nextDamageTime;

    private void Awake()
    {
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        if (_trailRenderer == null)
        {
            _trailRenderer = GetComponent<TrailRenderer>();
        }
        if (_trailRenderer == null)
        {
            _trailRenderer = gameObject.AddComponent<TrailRenderer>();
        }
    }

    private void SetupTrailRenderer()
    {
        _trailRenderer.time = _trailDuration;
        _trailRenderer.startWidth = _trailStartWidth;
        _trailRenderer.endWidth = _trailEndWidth;
        _trailRenderer.numCornerVertices = 5;
        _trailRenderer.numCapVertices = 5;
        _trailRenderer.minVertexDistance = 0.1f;
        
        if (_trailMaterial != null)
        {
            _trailRenderer.material = _trailMaterial;
        }
        
        _trailRenderer.sortingLayerName = "Default";
        _trailRenderer.sortingOrder = 9;
        
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] 
            { 
                new GradientColorKey(new Color(1f, 0.6f, 0f), 0f),
                new GradientColorKey(new Color(1f, 0.3f, 0f), 0.3f),
                new GradientColorKey(new Color(0.8f, 0.1f, 0f), 1f)
            },
            new GradientAlphaKey[] 
            { 
                new GradientAlphaKey(0.9f, 0f),
                new GradientAlphaKey(0.6f, 0.5f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        _trailRenderer.colorGradient = gradient;
        _trailRenderer.emitting = true;
    }

    public void Initialize(Transform weaponTransform, float duration, int damage)
    {
        _weaponTransform = weaponTransform;
        _trailDuration = duration;
        _damage = damage;
        _nextDamageTime = Time.time + _damageTickRate;

        SetupTrailRenderer();
    }

    private void Update()
    {
        if (_weaponTransform != null)
        {
            transform.position = _weaponTransform.position;
        }

        if (Time.time >= _nextDamageTime)
        {
            _damagedMonsters.Clear();
            _nextDamageTime = Time.time + _damageTickRate;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag(ConstTag.MONSTER))
        {
            BaseMonster monster = other.GetComponent<BaseMonster>();
            if (monster != null && !monster.isDead && !_damagedMonsters.Contains(monster))
            {
                monster.TakeDamage(_damage);
                _damagedMonsters.Add(monster);
            }
        }
    }
}