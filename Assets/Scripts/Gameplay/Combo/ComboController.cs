using System;
using UnityEngine;

public class ComboController : MonoBehaviour
{
    public static ComboController Instance { get; private set; }

    [Header("Combo Settings")]
    [SerializeField] private float _comboResetTime = 3f;
    [SerializeField] private int _scorePerCombo = 10;

    private int _currentCombo = 0;
    private float _comboTimer = 0f;
    private bool _isComboActive = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (_isComboActive)
        {
            _comboTimer -= Time.deltaTime;
            
            float normalizedTime = Mathf.Clamp01(_comboTimer / _comboResetTime);

            if (_comboTimer <= 0f)
            {
                ResetCombo();
            }
        }
    }

    public void AddCombo()
    {
        _currentCombo++;
        _comboTimer = _comboResetTime;
        _isComboActive = true;

        int bonusScore = _currentCombo * _scorePerCombo;
        
        var weapon = GameplayManager.Instance.weaponController.currentWeapon;
        if (weapon != null)
        {
            weapon.GainScore(bonusScore);
        }
    }

    public void ResetCombo()
    {
        if (_currentCombo > 0)
        {
            Debug.Log($"<color=orange>Combo ended at {_currentCombo}x</color>");
        }

        _currentCombo = 0;
        _comboTimer = 0f;
        _isComboActive = false;
    }

    public int GetCurrentCombo() => _currentCombo;
    public float GetComboTimer() => _comboTimer;
    public float GetComboTimerNormalized() => _isComboActive ? _comboTimer / _comboResetTime : 0f;
    public bool IsComboActive() => _isComboActive;
    public int GetComboScore() => _currentCombo * _scorePerCombo;

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}