using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerserkSkill : BaseWeaponSkill
{

    [Header("Berserk Settings")]
    [SerializeField] private float _berserkDuration = 5f;
    [SerializeField] private int _selfDamage = 1;
    [SerializeField] private float _damageMultiplier = 2f;
    [SerializeField] private float _critRateMultiplier = 2f;
    [SerializeField] private float _critDamageMultiplier = 2f;
    
    [Header("Visual Effects")]
    [SerializeField] private Color _berserkColor = new Color(1f, 0.2f, 0.2f, 1f);
    [SerializeField] private float _pulseSpeed = 15f;
    
    private Coroutine _berserkCoroutine;
    private bool _isSkillActivate = false;
    private Color _cachedOriginalColor;
    private WeaponState _cachedOriginalState;
    
    private int _originalAtk;
    
    public BerserkSkill(WeaponSkillData data) : base(data)
    {
    }

    public override void Activate(BaseWeapon weapon)
    {
        if (IsOnCooldown()) return;
        
        base.Activate(weapon);

        if (_berserkCoroutine != null)
        {
            weapon.StopCoroutine(_berserkCoroutine);
            ForceResetState(weapon);
        }

        _berserkCoroutine = weapon.StartCoroutine(IEBerserk(weapon));
    }

    private void ForceResetState(BaseWeapon weapon)
    {
        if (weapon == null) return;
        
        if (_isSkillActivate)
        {
            weapon.currentState = _cachedOriginalState;
            
            var spriteRenderer = weapon.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = _cachedOriginalColor;
            }
            
            weapon.data.atk = _originalAtk;
            
            _isSkillActivate = false;
            Debug.Log($"<color=magenta>Berserk Force Reset - Stats restored</color>");
        }
    }

    private IEnumerator IEBerserk(BaseWeapon weapon)
    {
        if (weapon == null) yield break;

        Debug.Log("<color=red>BERSERK MODE ACTIVATED!</color>");

        var spriteRenderer = weapon.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) yield break;

        if (!_isSkillActivate)
        {
            _cachedOriginalColor = spriteRenderer.color;
            _cachedOriginalState = weapon.currentState;
            
            _originalAtk = weapon.data.atk;
            
            _isSkillActivate = true;
        }

        weapon.currentState = WeaponState.SKILL_ACTIVE;

        weapon.TakeDamage(_selfDamage);
        Debug.Log($"<color=red>Berserk cost {_selfDamage} HP!</color>");

        weapon.data.atk = (int)(_originalAtk * _damageMultiplier);
        
        float elapsed = 0f;
        while (elapsed < _berserkDuration)
        {
            if (weapon == null) yield break;
            
            elapsed += Time.deltaTime;

            float pulse = Mathf.Abs(Mathf.Sin(elapsed * _pulseSpeed)) * 0.4f;
            Color currentColor = Color.Lerp(_berserkColor, Color.red, 0.6f + pulse);
            spriteRenderer.color = currentColor;

            yield return null;
        }

        Debug.Log("<color=green>Berserk mode ended!</color>");

        if (weapon != null)
        {
            ForceResetState(weapon);
        }

        _berserkCoroutine = null;
    }

    public bool IsBerserk()
    {
        return _isSkillActivate;
    }

    public float GetDamageMultiplier()
    {
        return _isSkillActivate ? _damageMultiplier : 1f;
    }
}
