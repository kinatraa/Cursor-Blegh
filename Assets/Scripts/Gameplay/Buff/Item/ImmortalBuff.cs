using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ImmortalBuff : BaseBuff
{
    private GameObject _currentShield;
    private Coroutine _shieldCycleCoroutine;
    public ImmortalBuff(BuffData data) : base(data)
    {
    }

    public override void AddStack()
    {
        base.AddStack();
        if (_shieldCycleCoroutine == null)
        {
            _shieldCycleCoroutine = CoroutineRunner.Instance.StartCoroutine(IEShieldCycle());
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        if (_shieldCycleCoroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(_shieldCycleCoroutine);
            _shieldCycleCoroutine = null;
        }

        if (_currentShield != null)
        {
            Object.Destroy(_currentShield);
        }
    }

    public IEnumerator IEShieldCycle()
    {
        while (true)
        {
            CreateShield();
            
            // thoi gian ton tai cua shield
            yield return new WaitForSeconds(2f);

            DestroyShield();

            yield return new WaitForSeconds(10f);
        }
    }

    private void CreateShield()
    {
        if (_currentShield != null)
        {
            Object.Destroy(_currentShield);
        }
        
        BaseWeapon weapon = GameplayManager.Instance.weaponController.currentWeapon;
        if (weapon == null) return;

        _currentShield = Object.Instantiate(data.prefab, weapon.transform);
        _currentShield.transform.localPosition = Vector3.zero;
        _currentShield.transform.localRotation = Quaternion.identity;
        
        Debug.Log("<color=cyan>Immortal Shield ACTIVATED!</color>");
    }

    private void DestroyShield()
    {
        if (_currentShield != null)
        {
            Object.Destroy(_currentShield);
            _currentShield = null;
            Debug.Log("<color=gray>Immortal Shield EXPIRED</color>");            
        }
    }
}
