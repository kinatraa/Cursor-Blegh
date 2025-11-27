using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : BaseMonsterProjectile
{
    protected override IEnumerator IEProjectileMove(Vector3 targetPosition)
    {
        yield return new WaitForSeconds(1f);
    }
}
