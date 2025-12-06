using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ImmortalObject : MonoBehaviour
{
    [Header("Shield Settings")]
    [SerializeField] private float shieldRadius = 0.5f;
    [SerializeField] private int segments = 60;
    [SerializeField] private float lineWidth = 0.15f;
    
    [Header("Colors")]
    [SerializeField] private Color innerColor = new Color(0.3f, 0.8f, 1f, 0.8f); 
    [SerializeField] private Color outerColor = new Color(0.5f, 0.9f, 1f, 0.3f); 
    
    [Header("Animation")]
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseAmount = 0.1f;
    [SerializeField] private float rotationSpeed = 30f;
    
    private LineRenderer _lineRenderer;
    private float _currentPulse = 0f;

    private void Awake()
    {
        SetupLineRenderer();
    }

    private void OnEnable()
    {
        StartCoroutine(IEAnimateShield());
    }

    private void SetupLineRenderer() {
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.startWidth = lineWidth;
        _lineRenderer.endWidth = lineWidth;
        
        var gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] 
            { 
                new GradientColorKey(innerColor, 0f),
                new GradientColorKey(outerColor, 0.5f),
                new GradientColorKey(innerColor, 1f)
            },
            new GradientAlphaKey[] 
            { 
                new GradientAlphaKey(innerColor.a, 0f),
                new GradientAlphaKey(outerColor.a, 0.5f),
                new GradientAlphaKey(innerColor.a, 1f)
            }
        );
        _lineRenderer.colorGradient = gradient;
        
        _lineRenderer.loop = true;
        _lineRenderer.useWorldSpace = false;
        _lineRenderer.sortingOrder = 10;
        _lineRenderer.numCapVertices = 5;
        _lineRenderer.numCornerVertices = 5;
        
        DrawCircle();
    }

    private void DrawCircle()
    {
        _lineRenderer.positionCount = segments + 1;
        
        float angleStep = 360f / segments;
        
        for (int i = 0; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * shieldRadius;
            float y = Mathf.Sin(angle) * shieldRadius;
            
            _lineRenderer.SetPosition(i, new Vector3(x, y, 0));
        }
    }

    private IEnumerator IEAnimateShield()
    {
        while (true)
        {
            _currentPulse += Time.deltaTime * pulseSpeed;
            float pulse = Mathf.Sin(_currentPulse) * pulseAmount;
            
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
            
            float angleStep = 360f / segments;
            for (int i = 0; i <= segments; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                float currentRadius = shieldRadius + pulse;
                float x = Mathf.Cos(angle) * currentRadius;
                float y = Mathf.Sin(angle) * currentRadius;
                
                _lineRenderer.SetPosition(i, new Vector3(x, y, 0));
            }
            
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(ConstTag.MONSTER_PROJECTILE))
        {
            Debug.Log("Collide with MONSTER_PROJECTILE");
            var projectile = other.GetComponent<BaseMonsterProjectile>();
            if (projectile != null && projectile.gameObject != null)
            {
                var projectileCollider = other.GetComponent<Collider2D>();
                if (projectileCollider != null)
                {
                    projectileCollider.enabled = false;
                }
                
                projectile.Destroy();
                
                StartCoroutine(IEFlashEffect());
            }
        }

        else if (other.CompareTag(ConstTag.MONSTER))
        {
            var weaponCollider = GameplayManager.Instance.weaponController.currentWeapon.GetComponent<Collider2D>();
            if (weaponCollider != null)
            {
                Physics2D.IgnoreCollision(other, weaponCollider);
            }
        }
    }

    private IEnumerator IEFlashEffect()
    {
        Color flashColor = Color.white;
        Gradient originalGradient = _lineRenderer.colorGradient;
        
        Gradient flashGradient = new Gradient();
        flashGradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(flashColor, 0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f) }
        );
        _lineRenderer.colorGradient = flashGradient;
        
        yield return new WaitForSeconds(0.1f);
        
        _lineRenderer.colorGradient = originalGradient;
    }
}