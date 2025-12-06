using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ImmortalBuff : BaseBuff
{
    private ImmortalObject _currentShield;
    private Coroutine _shieldCycleCoroutine;
    public ImmortalBuff(BuffData data) : base(data)
    {
    }

    public override void AddStack()
    {
        base.AddStack();
        CreateShield();
        _currentShield.gameObject.SetActive(false);
        if (_shieldCycleCoroutine == null)
        {
            _shieldCycleCoroutine = StartCoroutine(IEShieldCycle());
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        if (_shieldCycleCoroutine != null)
        {
            StopCoroutine(_shieldCycleCoroutine);
            _shieldCycleCoroutine = null;
        }

        DestroyShield();
    }

    public IEnumerator IEShieldCycle()
    {
        while (true)
        {
            GameplayManager.Instance.weaponController.currentWeapon.isImmortal = true;
            _currentShield.gameObject.SetActive(true);
            
            yield return new WaitForSeconds(2f);

            GameplayManager.Instance.weaponController.currentWeapon.isImmortal = false;
            _currentShield.gameObject.SetActive(false);

            yield return new WaitForSeconds(10f);
        }
    }

    private void CreateShield()
    {
        if (_currentShield != null)
        {
            Object.Destroy(_currentShield.gameObject);
        }
        
        BaseWeapon weapon = GameplayManager.Instance.weaponController.currentWeapon;
        if (weapon == null) return;

        _currentShield = Object.Instantiate(data.prefab, weapon.transform).GetComponent<ImmortalObject>();
        _currentShield.transform.localPosition = Vector3.zero;
        _currentShield.transform.localRotation = Quaternion.identity;
        
        Debug.Log("<color=cyan>Immortal Shield ACTIVATED!</color>");
    }

    private void DestroyShield()
    {
        if (_currentShield != null)
        {
            Object.Destroy(_currentShield.gameObject);
            _currentShield = null;
            Debug.Log("<color=gray>Immortal Shield EXPIRED</color>");            
        }
    }
}
