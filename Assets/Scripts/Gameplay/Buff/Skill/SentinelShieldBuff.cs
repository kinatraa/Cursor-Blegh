using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentinelShieldBuff : BaseBuff
{
    private List<GameObject> _orbs = new List<GameObject>();
    private Transform _holder;
    private float _radius = 1.5f;       
    private float _rotateSpeed = 180f; //degree per seconds

    public SentinelShieldBuff(BuffData data) : base(data)
    {
    }

    public override void AddStack()
    {
        base.AddStack();
        
        if (!_holder) _holder = GameplayManager.Instance.weaponController.currentWeapon.transform;

        var newOrb = Object.Instantiate(data.prefab, _holder);
        newOrb.transform.localPosition = Vector3.zero;

        _orbs.Add(newOrb);

        UpdateOrbAngles();
        
        string hitKey = "buff_shield";
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ShotSfx(hitKey, pitch: 2f);
        }
        
        StartCoroutine(IEOrbit());
    }

    private void UpdateOrbAngles()
    {
        int count = _orbs.Count;
        for (int i = 0; i < count; i++)
        {
            float angle = (360f / count) * i;
            _orbs[i].transform.localPosition = AngleToLocalPos(angle);
        }
    }

    private Vector3 AngleToLocalPos(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * _radius;
    }

    private IEnumerator IEOrbit()
    {
        float orbitAngle = 0f;

        while (true)
        {
            orbitAngle += _rotateSpeed * Time.deltaTime;

            int count = _orbs.Count;
            for (int i = 0; i < count; i++)
            {
                float angle = orbitAngle + (360f / count) * i;
                _orbs[i].transform.localPosition = AngleToLocalPos(angle);
            }

            yield return null;
        }
    }
}