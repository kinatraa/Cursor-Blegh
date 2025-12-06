using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChooseWeaponButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public WeaponData data;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.Instance.uiChooseWeapon.ShowDescription(data);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.uiChooseWeapon.HideDescription();
    }
}