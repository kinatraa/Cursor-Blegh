using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthSkill : BaseWeaponSkill
{
    [Header("Stealth Settings")]
    [SerializeField] private float _stealthDuration = 3f;
    [SerializeField] private float _fadeAlpha = 0.3f;
    [SerializeField] private float _fadeInDuration = 0.3f;
    [SerializeField] private float _fadeOutDuration = 0.3f;
    [SerializeField] private bool _showStealthParticles = true;

    private Coroutine _stealthCoroutine;
    private bool _isSkillActivate = false;
    private Color _cachedOriginalColor;
    private WeaponState _cachedOriginalState;


    public StealthSkill(WeaponSkillData data) : base(data)
    {
    }

    public override void Activate(BaseWeapon weapon)
    {
        if (IsOnCooldown()) return;
        
        base.Activate(weapon);

        if (_stealthCoroutine != null)
        {
            weapon.StopCoroutine(_stealthCoroutine);
            ForceResetState(weapon);
        }
        
        string hitKey = "sfx_disappear";
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ShotSfx(hitKey);
        }

        _stealthCoroutine = weapon.StartCoroutine(IEStealth(weapon));
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

            _isSkillActivate = false;
            Debug.Log($"Reset Weapon State: {weapon.currentState}");
        }
    }

    private IEnumerator IEStealth(BaseWeapon weapon){
        if (weapon == null) yield break;

        Debug.Log("<color=blue>Stealth activated! Becoming invisible...</color>");

        var spriteRenderer = weapon.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) yield break;

        if (!_isSkillActivate)
        {
            _cachedOriginalColor = spriteRenderer.color;
            _cachedOriginalState = weapon.currentState;
            _isSkillActivate = true;
        }

        weapon.currentState = WeaponState.SKILL_ACTIVE;

        yield return FadeToStealth(spriteRenderer, _cachedOriginalColor);

        float elapsed = 0f;
        while (elapsed < _stealthDuration)
        {
            elapsed += Time.deltaTime;

            float pulse = Mathf.Abs(Mathf.Sin(elapsed * 5f)) * 0.1f;
            Color currentColor = spriteRenderer.color;
            currentColor.a = _fadeAlpha + pulse;
            spriteRenderer.color = currentColor;

            yield return null;
        }

        if (weapon != null)
        {
            yield return FadeFromStealth(spriteRenderer, _cachedOriginalColor);
            ForceResetState(weapon);
            Debug.Log("<color=green>Stealth ended! Visible again.</color>");
        }

        _stealthCoroutine = null;
    }

    private IEnumerator FadeToStealth(SpriteRenderer sr, Color originalColor)
    {
        float elapsed = 0f;
        Color startColor = sr.color;
        Color targetColor = new Color(
            originalColor.r,
            originalColor.g,
            originalColor.b,
            _fadeAlpha
        );
        while (elapsed < _fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _fadeInDuration;
            sr.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }
        sr.color = targetColor;
    }

    private IEnumerator FadeFromStealth(SpriteRenderer sr, Color originalColor)
    {
        float elapsed = 0f;
        Color startColor = sr.color;

        while (elapsed < _fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _fadeOutDuration;
            sr.color = Color.Lerp(startColor, originalColor, t);
            yield return null;
        }
        sr.color = originalColor;
    }
}
