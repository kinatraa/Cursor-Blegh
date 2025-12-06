    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class SpinSplashSkill : BaseWeaponSkill
    {

        [Header("Spin Splash Settings")]
        [SerializeField] private float _spinDuration = 1f;
        [SerializeField] private float _spinSpeed = 360f;
        [SerializeField] private float _projectileDestroyRadius = 3f;
        [SerializeField] private int _numberOfSpins = 1;
        [SerializeField] private float _checkInterval = 0.05f;
        
        [Header("Visual Effects")]
        [SerializeField] private Color _spinColor = new Color(1f ,1f, 1f, 1f);
        [SerializeField] private bool _showDebugRadius = true;
        [SerializeField] private int _circleSegments = 50;
        [SerializeField] private float _circleLineWidth = 0.1f;
        
        private Coroutine _spinCoroutine;
        private bool _isSkillActivate = false;
        private Quaternion _cachedOriginalRotation;
        private Color _cachedOriginalColor;
        private WeaponState _cachedOriginalState;

        public SpinSplashSkill(WeaponSkillData data) : base(data)
        {
        }

        public override void Activate(BaseWeapon weapon)
        {
            if (IsOnCooldown()) return;
            
            base.Activate(weapon);

            if (_spinCoroutine != null)
            {
                weapon.StopCoroutine(_spinCoroutine);
                ForceResetState(weapon);
            }
            
            _spinCoroutine = weapon.StartCoroutine(IESpinSplash(weapon));
        }

        private void ForceResetState(BaseWeapon weapon)
        {
            if (weapon == null) return;
            if (_isSkillActivate)
            {
                weapon.transform.localRotation = Quaternion.identity;
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

        private IEnumerator IESpinSplash(BaseWeapon weapon)
        {
            if (weapon == null) yield break;
            
            Debug.Log("<color=cyan>SpinSplash activated!</color>");

            if (!_isSkillActivate)
            {
                _cachedOriginalColor = weapon.GetComponent<SpriteRenderer>()?.color ??  Color.white;
                _cachedOriginalState = weapon.currentState;
                _isSkillActivate = true;
            }
            
            weapon.currentState = WeaponState.SKILL_ACTIVE;
            
            float elapsed = 0f;
            float lastCheckTime = 0f;
            int projectilesDestroyed = 0;
            
            var spriteRenderer = weapon.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = _spinColor;
            }
            
            while (elapsed < _spinDuration)
            {
                if (weapon == null) yield break;
                
                elapsed += Time.deltaTime;
                
                float rotationAmount = _spinSpeed * Time.deltaTime;
                weapon.transform.Rotate(0, 0, rotationAmount);
                
                if (elapsed - lastCheckTime >= _checkInterval)
                {
                    projectilesDestroyed += DestroyProjectilesInRadius(weapon.transform.position, _projectileDestroyRadius);
                    lastCheckTime = elapsed;
                }
                
                yield return null;
            }

            if (weapon != null)
            {
                projectilesDestroyed += DestroyProjectilesInRadius(weapon.transform.position, _projectileDestroyRadius);
                Debug.Log($"<color=green>SpinSplash destroyed {projectilesDestroyed} projectiles!</color>");
                ForceResetState(weapon);
            }
            
            _spinCoroutine = null;
        }
        
        private int DestroyProjectilesInRadius(Vector3 center, float radius)
        {
            int destroyedCount = 0;
            
            Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius);
            
            foreach (var hit in hits)
            {
                if (hit.CompareTag(ConstTag.MONSTER_PROJECTILE))
                {
                    var projectile = hit.GetComponent<BaseMonsterProjectile>();
                    if (projectile != null)
                    {
                        if (projectile is LaserBeam)
                        {
                            continue;
                        }
                        
                        projectile.Destroy();
                        destroyedCount++;
                        
                        Debug.Log($"<color=yellow>Destroyed projectile: {projectile.GetType().Name}</color>");
                    }
                }
            }
            
            return destroyedCount;
        }
    }