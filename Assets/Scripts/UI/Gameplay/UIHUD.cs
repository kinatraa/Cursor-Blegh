using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHUD : MonoBehaviour
{
    [Header("Health Bar")]
    public List<RectTransform> heartIcons = new List<RectTransform>();

    public void SetupHealthBar(int maxHp)
    {
        for (int i = 0; i < heartIcons.Count; i++)
        {
            if (i < maxHp)
            {
                heartIcons[i].gameObject.SetActive(true);
                heartIcons[i].transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                heartIcons[i].gameObject.SetActive(false);
            }
        }
    }
    
    public void UpdateHealthBar(int hp)
    {
        for (int i = 0; i < heartIcons.Count; i++)
        {
            if (i < hp)
            {
                heartIcons[i].transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                heartIcons[i].transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }
}
