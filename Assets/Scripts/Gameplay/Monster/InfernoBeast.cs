using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfernoBeast : BaseMonster
{
    protected override IEnumerator IEAttackPlayer()
    {
        _sr.color = Color.red;
        yield return new WaitForSeconds(1f);
    }
}
