using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderBeast : BaseMonster
{
    [Header("Thunder Attack Settings")]
    [SerializeField] private float _electricFieldDuration = 2f;
    [SerializeField] private float _fieldRadiusMultiplier = 3f;
    [SerializeField] private float _damageInterval = 0.2f;
    [SerializeField] private Color _electricColor = new Color(0.5f, 0.8f, 1f, 0.5f);
    
    [Header("Visual Circle Settings")]
    [SerializeField] private int _circleSegments = 100;
    [SerializeField] private float _circleLineWidth = 0.1f;
    [SerializeField] private bool _animateCircle = true;
    [SerializeField] private float _pulseSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 30f;
    
    private GameObject _electricCircle;
    private LineRenderer _circleLineRenderer;
    private bool _isElectricFieldActive = false;
    private float _currentFieldRadius = 0f;
    private HashSet<BaseWeapon> _damagedWeaponsThisInterval = new HashSet<BaseWeapon>();
    
    protected override IEnumerator IEAttackPlayer()
    {
        string hitKey = "monster_thunder";
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ShotSfx(hitKey);
        }
        
        PlayAnimation(ANIM_ATTACK);
        
        yield return StartCoroutine(IEActivateElectricField());
        
        yield return new WaitForSeconds(_remainingAnimTime);
        
        _sr.color = Color.white;
    }
    
    private IEnumerator IEActivateElectricField()
    {
        float monsterRadius = GetMonsterRadius();
        _currentFieldRadius = monsterRadius * _fieldRadiusMultiplier;
        _isElectricFieldActive = true;
        
        CreateElectricCircle();
        
        Debug.Log($"<color=cyan>ThunderBeast activated electric field with radius {_currentFieldRadius}</color>");
        
        float elapsed = 0f;
        float lastDamageTime = 0f;
        
        while (elapsed < _electricFieldDuration)
        {
            elapsed += Time.deltaTime;
            
            if (elapsed - lastDamageTime >= _damageInterval)
            {
                DealDamageInField();
                lastDamageTime = elapsed;
                _damagedWeaponsThisInterval.Clear();
            }
            
            if (_animateCircle && _circleLineRenderer != null)
            {
                AnimateElectricCircle(elapsed);
            }
            
            yield return null;
        }
        
        DeactivateElectricField();
    }
    
    private void CreateElectricCircle()
    {
        _electricCircle = new GameObject("ElectricCircle");
        _electricCircle.transform.SetParent(transform);
        _electricCircle.transform.localPosition = Vector3.zero;
        
        _circleLineRenderer = _electricCircle.AddComponent<LineRenderer>();
        
        _circleLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _circleLineRenderer.startColor = _electricColor;
        _circleLineRenderer.endColor = _electricColor;
        _circleLineRenderer.startWidth = _circleLineWidth;
        _circleLineRenderer.endWidth = _circleLineWidth;
        _circleLineRenderer.positionCount = _circleSegments + 1;
        _circleLineRenderer.useWorldSpace = false;
        _circleLineRenderer.loop = true;
        
        DrawCircle(_currentFieldRadius);
    }
    
    private void DrawCircle(float radius)
    {
        if (_circleLineRenderer == null) return;
        
        float angleStep = 360f / _circleSegments;
        
        for (int i = 0; i <= _circleSegments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            
            _circleLineRenderer.SetPosition(i, new Vector3(x, y, 0));
        }
    }
    
    private void AnimateElectricCircle(float time)
    {
        if (_circleLineRenderer == null) return;
        
        float pulse = Mathf.PingPong(time * _pulseSpeed, 1f);
        float width = _circleLineWidth * (1f + pulse * 0.5f);
        _circleLineRenderer.startWidth = width;
        _circleLineRenderer.endWidth = width;
        
        Color c = _electricColor;
        c.a = 0.3f + pulse * 0.4f;
        _circleLineRenderer.startColor = c;
        _circleLineRenderer.endColor = c;
        
        _electricCircle.transform.Rotate(Vector3.forward, _rotationSpeed * Time.deltaTime);
        
        float angleStep = 360f / _circleSegments;
        for (int i = 0; i <= _circleSegments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float wave = Mathf.Sin(time * 3f + i * 0.5f) * 0.1f;
            float radiusWithWave = _currentFieldRadius * (1f + wave);
            
            float x = Mathf.Cos(angle) * radiusWithWave;
            float y = Mathf.Sin(angle) * radiusWithWave;
            
            _circleLineRenderer.SetPosition(i, new Vector3(x, y, 0));
        }
    }
    
    private void DealDamageInField()
    {
        if (!_isElectricFieldActive) return;
        
        var weaponController = GameplayManager.Instance.weaponController;
        if (weaponController == null) return;
        
        var currentWeapon = weaponController.currentWeapon;
        if (currentWeapon == null) return;
        
        float distance = Vector3.Distance(transform.position, currentWeapon.transform.position);
        
        if (distance <= _currentFieldRadius)
        {
            if (!_damagedWeaponsThisInterval.Contains(currentWeapon))
            {
                CombatResolver.MonsterDamageWeapon(currentWeapon, this);
                _damagedWeaponsThisInterval.Add(currentWeapon);
                
                Debug.Log($"<color=yellow>ThunderBeast zapped {currentWeapon.name} (distance: {distance:F2})</color>");
            }
        }
    }
    
    private void DeactivateElectricField()
    {
        _isElectricFieldActive = false;
        _currentFieldRadius = 0f;
        
        if (_electricCircle != null)
        {
            Destroy(_electricCircle);
            _electricCircle = null;
            _circleLineRenderer = null;
        }
        
        _damagedWeaponsThisInterval.Clear();
        
        Debug.Log("<color=cyan>ThunderBeast deactivated electric field</color>");
    }
    
    protected override void Die()
    {
        DeactivateElectricField();
        base.Die();
    }
}