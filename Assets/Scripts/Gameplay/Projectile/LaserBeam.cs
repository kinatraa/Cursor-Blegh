using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : BaseMonsterProjectile
{
    [Header("Laser Settings")]
    [SerializeField] private float _chargeDuration = 1f; 
    [SerializeField] private float _fireDuration = 2f;
    [SerializeField] private float _dissolveDuration = 0.5f; 
    [SerializeField] private float _laserLength = 20f; 
    [SerializeField] private float _laserWidth = 0.5f;
    
    [Header("Visual")]
    [SerializeField] private GameObject _chargeEffect; 
    [SerializeField] private Color _chargeColor = Color.yellow;
    [SerializeField] private Color _fireColor = Color.red;
    
    [Header("Damage")]
    [SerializeField] private float _damageInterval = 0.1f;
    
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _laserCollider;
    private Vector3 _fireDirection;
    private bool _isFiring = false;
    
    private HashSet<BaseWeapon> _damagedWeaponsThisFrame = new HashSet<BaseWeapon>();
    private float _lastDamageTime = 0f;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _laserCollider = GetComponent<BoxCollider2D>();
        
        if (_laserCollider == null)
        {
            _laserCollider = gameObject.AddComponent<BoxCollider2D>();
            _laserCollider.isTrigger = true;
        }
    }

    public void Initialize(Vector3 startPos, Vector3 direction, float length)
    {
        transform.position = startPos;
        _fireDirection = direction.normalized;
        _laserLength = length;
        
        // Tính góc quay
        float angle = Mathf.Atan2(_fireDirection.y, _fireDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        
        StartCoroutine(IELaserSequence());
    }

    protected override IEnumerator IEProjectileMove(Vector3 targetPosition)
    {
        yield break;
    }

    private IEnumerator IELaserSequence()
    {
        yield return StartCoroutine(IEChargePhase());
        
        yield return StartCoroutine(IEFirePhase());
        
        yield return StartCoroutine(IEDissolvePhase());
        
        Destroy();
    }

    private IEnumerator IEChargePhase()
    {
        if (_chargeEffect != null)
            _chargeEffect.SetActive(true);
        
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = _chargeColor;
            _spriteRenderer.size = new Vector2(_laserLength, _laserWidth * 0.3f); // Mỏng hơn khi charge
        }
        
        if (_laserCollider != null)
            _laserCollider.enabled = false;
        
        float elapsed = 0f;
        while (elapsed < _chargeDuration)
        {
            elapsed += Time.deltaTime;
            float pulse = Mathf.PingPong(elapsed * 4f, 1f);
            
            if (_spriteRenderer != null)
            {
                Color c = _chargeColor;
                c.a = 0.3f + pulse * 0.7f;
                _spriteRenderer.color = c;
            }
            
            yield return null;
        }
        
        if (_chargeEffect != null)
            _chargeEffect.SetActive(false);
    }

    private IEnumerator IEFirePhase()
    {
        _isFiring = true;
        
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = _fireColor;
            _spriteRenderer.size = new Vector2(_laserLength, _laserWidth);
        }
        
        if (_laserCollider != null)
        {
            _laserCollider.enabled = true;
            _laserCollider.size = new Vector2(_laserLength, _laserWidth);
            _laserCollider.offset = new Vector2(_laserLength / 2, 0);
        }
        
        float timer = 0f;
        _lastDamageTime = 0f;
        
        while (timer < _fireDuration)
        {
            timer += Time.deltaTime;
            
            // Deal damage on interval
            if (timer - _lastDamageTime >= _damageInterval)
            {
                DealDamageToWeaponsInRange();
                _lastDamageTime = timer;
                _damagedWeaponsThisFrame.Clear();
            }
            
            yield return null;
        }
        
        _isFiring = false;
    }

    private IEnumerator IEDissolvePhase()
    {
        // Disable collider
        if (_laserCollider != null)
            _laserCollider.enabled = false;
        
        // Fade out
        float elapsed = 0f;
        Color startColor = _spriteRenderer != null ? _spriteRenderer.color : Color.white;
        
        while (elapsed < _dissolveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _dissolveDuration;
            
            if (_spriteRenderer != null)
            {
                Color c = startColor;
                c.a = Mathf.Lerp(1f, 0f, t);
                _spriteRenderer.color = c;
            }
            
            yield return null;
        }
    }

    private void DealDamageToWeaponsInRange()
    {
        if (_laserCollider == null || !_laserCollider.enabled) return;
        
        Vector3 boxCenter = transform.position + transform.right * (_laserLength / 2);
        Vector2 boxSize = new Vector2(_laserLength, _laserWidth);
        
        // Detect overlap
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            boxCenter,
            boxSize,
            transform.eulerAngles.z
        );
        
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                var weapon = hit.GetComponent<BaseWeapon>();
                if (weapon != null && !_damagedWeaponsThisFrame.Contains(weapon))
                {
                    CombatResolver.CollisionResolve(weapon, this);
                    _damagedWeaponsThisFrame.Add(weapon);
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!_isFiring) return;
        
        if (collision.CompareTag("Player"))
        {
            var weapon = collision.GetComponent<BaseWeapon>();
            if (weapon != null && !_damagedWeaponsThisFrame.Contains(weapon))
            {
                CombatResolver.CollisionResolve(weapon, this);
                _damagedWeaponsThisFrame.Add(weapon);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        
        Gizmos.color = _isFiring ? Color.red : Color.yellow;
        
        Vector3 start = transform.position;
        Vector3 end = transform.position + transform.right * _laserLength;
        
        Gizmos.DrawLine(start, end);
        
        if (_laserCollider != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(_laserCollider.offset, _laserCollider.size);
        }
    }
}