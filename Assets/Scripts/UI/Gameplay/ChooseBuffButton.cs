using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChooseBuffButton : MonoBehaviour
{
    public BuffType type;
    public Image image;
    public TextMeshProUGUI level;
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;

    private BuffData _buffData;
    
    public void Setup(BuffData data)
    {
        _buffData = data;
        type = data.type;
        image.sprite = data.buffIcon;
        if (data.effectType == BuffEffectType.SKILL)
        {
            level.text = $"Level {GameplayManager.Instance.buffController.GetBuffCurrentStack(type) + 1}";
            level.gameObject.SetActive(true);
        }
        else
        {
            level.gameObject.SetActive(false);
        }
        title.text = data.buffName;
        description.text = data.description;
    }

    public void Choose()
    {
        GameEventManager.InvokeChooseBuff(_buffData);
        UIManager.Instance.chooseUpgradePopup.Hide();
    }
}
