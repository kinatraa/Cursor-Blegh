using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffSlot : MonoBehaviour
{
    public BuffData buffData;
    public Image image;
    public TextMeshProUGUI level;

    private void Awake()
    {
        image = GetComponent<Image>();
        level = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    public void Setup(BuffData data)
    {
        buffData = data;
        image.sprite = buffData.buffIcon;
        AddStack();
    }

    public void AddStack()
    {
        if (buffData.maxStack <= 1)
        {
            level.gameObject.SetActive(false);
            return;
        }
        
        int lvl = GameplayManager.Instance.buffController.GetBuffCurrentStack(buffData.type);
        if (lvl > 0)
        {
            level.text = $"{lvl}";
            level.gameObject.SetActive(true);
        }
        else
        {
            level.gameObject.SetActive(false);
        }
    }

    public void Reset()
    {
        buffData = null;
        gameObject.SetActive(false);
    }
}
