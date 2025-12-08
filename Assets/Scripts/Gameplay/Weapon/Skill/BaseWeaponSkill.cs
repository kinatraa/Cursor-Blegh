using UnityEngine;

public abstract class BaseWeaponSkill
{
    protected WeaponSkillData data;

    private float _lastActiveTime = -999f;
    private bool _isOnCooldown = false;

    public BaseWeaponSkill(WeaponSkillData data)
    {
        this.data = data;
    }

    public virtual void Activate(BaseWeapon owner)
    {
        if (IsOnCooldown())
        {
            float remainingCooldown = GetRemainingCooldown();
            Debug.Log($"<color=yellow>Skill {data.type} on cooldown! {remainingCooldown:F1}s remaining</color>");
            return;
        }
        
        _lastActiveTime = Time.time;
        _isOnCooldown = true;
        
        Debug.Log($"Data : {data.type}, {data.cooldown}, {data.range}, {data.duration}");
    }

    public float GetRemainingCooldown()
    {
        if (!_isOnCooldown) return 0f;
        
        float elapsed = Time.time - _lastActiveTime;
        float remainingCooldown =Mathf.Max(0f, data.cooldown - elapsed);
        return remainingCooldown;
    }

    public bool IsOnCooldown()
    {
        if (!_isOnCooldown) return false;

        float timeSinceActivate = Time.time - _lastActiveTime;

        if (timeSinceActivate >= data.cooldown)
        {
            _isOnCooldown = false;
            return false;
        }
        
        string hitKey = "item_hover";
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ShotSfx(hitKey, 2f, 0.5f);
        }
        
        return true;
    }
}