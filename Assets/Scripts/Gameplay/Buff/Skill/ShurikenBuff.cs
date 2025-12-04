using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShurikenBuff : BaseBuff
{
    public ShurikenBuff(BuffData data) : base(data)
    {
    }

    public override void Activate()
    {
        if (Random.value * 100 >= data.activeRate) return;

        StartCoroutine(IEActivate());
    }

    private IEnumerator IEActivate()
    {
        List<BaseMonster> monsters = GetSortedMonstersFromWeapon(GameplayManager.Instance.monsterController.MonsterList);
        if (monsters.Count == 0) yield break;

        GameObject shuriken = Object.Instantiate(data.prefab, monsters[0].transform.position, Quaternion.identity);

        int loop = stack;
        while (loop-- > 0)
        {
            monsters = GetSortedMonstersByPath(GameplayManager.Instance.monsterController.MonsterList, shuriken.transform.position);

            foreach (var monster in monsters)
            {
                if (!monster) continue;
                
                yield return StartCoroutine(IEMoveShuriken(shuriken.transform, monster.transform, 10f));
                if (!monster) continue;
                monster.TakeDamage(data.atk);
            }
        }

        Object.Destroy(shuriken);
    }

    private List<BaseMonster> GetSortedMonstersFromWeapon(List<BaseMonster> monsters)
    {
        Vector3 weaponPos = GameplayManager.Instance.weaponController.currentWeapon.transform.position;
        return GetSortedMonstersByPath(monsters, weaponPos);
    }

    private List<BaseMonster> GetSortedMonstersByPath(List<BaseMonster> monsters, Vector3 startPos)
    {
        List<BaseMonster> sortedMonsters = new List<BaseMonster>();
        List<BaseMonster> remainingMonsters = new List<BaseMonster>(monsters);

        BaseMonster current = remainingMonsters.OrderBy(m => Vector3.Distance(startPos, m.transform.position)).FirstOrDefault();
        if (current == null) return sortedMonsters;

        sortedMonsters.Add(current);
        remainingMonsters.Remove(current);

        while (remainingMonsters.Count > 0)
        {
            BaseMonster next = remainingMonsters.OrderBy(m => Vector3.Distance(current.transform.position, m.transform.position)).FirstOrDefault();
            if (next == null) break;

            sortedMonsters.Add(next);
            remainingMonsters.Remove(next);
            current = next;
        }

        return sortedMonsters;
    }

    private IEnumerator IEMoveShuriken(Transform shuriken, Transform target, float speed)
    {
        var targetPos = target.position;
        while (Vector3.Distance(shuriken.position, targetPos) > 0.01f)
        {
            if (!target)
            {
                yield break;
            }

            shuriken.position = Vector3.MoveTowards(
                shuriken.position,
                targetPos,
                speed * Time.deltaTime
            );

            yield return null;
        }
    }
}