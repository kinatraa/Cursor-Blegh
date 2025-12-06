using UnityEngine;

public class WaveRewardSystem : MonoBehaviour
{
    [Header("Wave Reward Settings")]
    [SerializeField] private int _completionBonus = 100;
    [SerializeField] private int _perfectBonus = 500;

    private int _startWaveHP = 0;
    private bool _hasTakenDamage = false;

    private void OnEnable()
    {
        GameEventManager.onNextWave += OnWaveComplete;
    }

    private void OnDisable()
    {
        GameEventManager.onNextWave -= OnWaveComplete;
    }

    public void StartWave()
    {
        var weapon = GameplayManager.Instance.weaponController.currentWeapon;
        if (weapon != null)
        {
            _startWaveHP = weapon.currentHp;
            _hasTakenDamage = false;
            Debug.Log($"<color=cyan>Wave started - HP: {_startWaveHP}</color>");
        }
    }

    public void OnDamageTaken()
    {
        _hasTakenDamage = true;
    }

    private void OnWaveComplete()
    {
        var weapon = GameplayManager.Instance.weaponController.currentWeapon;
        if (weapon == null) return;

        int totalBonus = _completionBonus;
        bool isPerfect = !_hasTakenDamage && weapon.currentHp == _startWaveHP;

        if (isPerfect)
        {
            totalBonus += _perfectBonus;
            Debug.Log($"<color=yellow>★★★ PERFECT WAVE! ★★★</color>");
        }

        weapon.GainScore(totalBonus);

        string bonusDetail = isPerfect 
            ? $"{_completionBonus} (Complete) + {_perfectBonus} (Perfect)" 
            : $"{_completionBonus} (Complete)";
        
        Debug.Log($"<color=green>Wave Bonus: {bonusDetail} = +{totalBonus} points</color>");

        // Reset cho wave mới
        StartWave();
    }

    public bool IsPerfectWave()
    {
        var weapon = GameplayManager.Instance.weaponController.currentWeapon;
        return weapon != null && !_hasTakenDamage && weapon.currentHp == _startWaveHP;
    }
}