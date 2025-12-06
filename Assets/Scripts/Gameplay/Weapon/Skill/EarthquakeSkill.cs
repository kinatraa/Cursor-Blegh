using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthquakeSkill : BaseWeaponSkill
{

    [Header("Earthquake Settings")]
    [SerializeField] private float _damageRadius = 3f;
    [SerializeField] private float _damageMultiplier = 3f;
    [SerializeField] private int _selfDamage = 1;

    [Header("Visual Effects")]
    [SerializeField] private Color _impactColor = new Color(1f, 0f, 0f, 0.5f);
    [SerializeField] private int _shockwaveSegments = 50;
    [SerializeField] private float _shockwaveLineWidth = 0.15f;
    [SerializeField] private float _shockwaveExpandDuration = 0.5f;
    
    private Coroutine _slamCoroutine;
    private bool _isSkillActivate = false;
    private WeaponState _cachedOriginalState;
    
    public EarthquakeSkill(WeaponSkillData data) : base(data)
    {
    }

    public override void Activate(BaseWeapon weapon)
    {
        if (IsOnCooldown()) return;
        
        base.Activate(weapon);

        if (_slamCoroutine != null)
        {
            weapon.StopCoroutine(_slamCoroutine);
            ForceResetState(weapon);
        }
        
        string hitKey = "sfx_groundslam";
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ShotSfx(hitKey, 5f, 0.5f);
        }

        _slamCoroutine = weapon.StartCoroutine(IEGroundSlam(weapon));
    }

    private void ForceResetState(BaseWeapon weapon)
    {
        if (weapon == null) return;
        
        if (_isSkillActivate)
        {
            weapon.currentState = _cachedOriginalState;
            
            _isSkillActivate = false;
        }
    }

    private IEnumerator IEGroundSlam(BaseWeapon weapon)
    {
        if (weapon == null) yield break;

        Debug.Log("<color=orange>Earthquake activated</color>");

        if (!_isSkillActivate)
        {
            _cachedOriginalState = weapon.currentState;
            _isSkillActivate = true;
        }

        weapon.currentState = WeaponState.SKILL_ACTIVE;

        int monstersHit = DamageEnemiesInRadius(weapon);

        weapon.TakeDamage(_selfDamage);
        Debug.Log($"<color=red>Battle Axe took {_selfDamage} self-damage!</color>");

        yield return IEShowShockwave(weapon.transform.position);

        Debug.Log($"<color=green>Earthquake hit {monstersHit} monsters!</color>");

        if (weapon != null)
        {
            ForceResetState(weapon);
        }

        _slamCoroutine = null;
    }

    private int DamageEnemiesInRadius(BaseWeapon weapon)
    {
        int hitCount = 0;
        Collider2D[] hits = Physics2D.OverlapCircleAll(weapon.transform.position, _damageRadius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag(ConstTag.MONSTER))
            {
                var monster = hit.GetComponent<BaseMonster>();
                if (monster != null)
                {
                    int baseDamage = weapon.data.atk;
                    int totalDamage = (int)(baseDamage * _damageMultiplier);
                    
                    if (Random.Range(0, 100) < weapon.data.crit + weapon.critChanceToAdd)
                    {
                        totalDamage = (int)(totalDamage * ((weapon.data.critDmg + weapon.critDmgToAdd) / 100f));
                        Debug.Log($"<color=orange>CRITICAL EARTHQUAKE!</color>");
                    }

                    monster.TakeDamage(totalDamage);
                    hitCount++;
                    
                    Debug.Log($"<color=yellow>Earthquake dealt {totalDamage} damage to {monster.name}</color>");
                }
            }
        }

        return hitCount;
    }

    private IEnumerator IEShowShockwave(Vector3 center)
    {
        // Create shockwave object
        GameObject shockwaveObj = new GameObject("Shockwave");
        shockwaveObj.transform.position = center;

        LineRenderer lineRenderer = shockwaveObj.AddComponent<LineRenderer>();
        lineRenderer.positionCount = _shockwaveSegments + 1;
        lineRenderer.startWidth = _shockwaveLineWidth;
        lineRenderer.endWidth = _shockwaveLineWidth;
        lineRenderer.useWorldSpace = true;
        lineRenderer.loop = true;

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = _impactColor;
        lineRenderer.endColor = _impactColor;

        float elapsed = 0f;
        float startRadius = 0.5f;
        
        while (elapsed < _shockwaveExpandDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _shockwaveExpandDuration;
            
            float currentRadius = Mathf.Lerp(startRadius, _damageRadius, t);
            
            float alpha = 1f - t;

            float angleStep = 360f / _shockwaveSegments;
            for (int i = 0; i <= _shockwaveSegments; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                float x = Mathf.Cos(angle) * currentRadius;
                float y = Mathf.Sin(angle) * currentRadius;
                lineRenderer.SetPosition(i, center + new Vector3(x, y, 0));
            }

            Color color = _impactColor;
            color.a = alpha;
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;

            yield return null;
        }

        // Cleanup
        Object.Destroy(shockwaveObj);
    }
}
