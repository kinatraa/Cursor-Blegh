using UnityEngine;

public class HealingHerbObject : MonoBehaviour
{
    private int _healAmount = 1;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(ConstTag.WEAPON))
        {
            string hitKey = "item_choose";
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.ShotSfx(hitKey, pitch: 2f);
            }
            var weapon = GameplayManager.Instance.weaponController.currentWeapon;
            weapon.HealByItem(_healAmount);
            
            Destroy(gameObject);
        }
    }
}
