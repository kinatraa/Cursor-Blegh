using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoltenStoneBuff : BaseBuff
{
    
    private GameObject _currentMoltenTrail;
    
    public MoltenStoneBuff(BuffData data) : base(data)
    {
    }

    public override void AddStack()
    {
        base.AddStack();
        Activate();
    }

    public override void Activate()
    {
        if (_currentMoltenTrail != null)
        {
            Object.Destroy(_currentMoltenTrail);
        }

        BaseWeapon currentWeapon = GameplayManager.Instance.weaponController.currentWeapon;
        if (currentWeapon == null) return;

        float trailDuration = 1f + (stack - 1) * 0.2f;

        _currentMoltenTrail = Object.Instantiate(
            data.prefab,
            currentWeapon.transform.position,
            Quaternion.identity
        );

        MoltenStoneObject moltenObject = _currentMoltenTrail.GetComponent<MoltenStoneObject>();
        if (moltenObject != null)
        {
            moltenObject.Initialize(currentWeapon.transform, trailDuration, data.atk);
        }
    }
}
