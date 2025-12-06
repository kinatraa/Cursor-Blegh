using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChooseUpgradePopup : UIPopup
{
    public List<ChooseOptionButton> optionButtons = new List<ChooseOptionButton>();
    public TextMeshProUGUI rerollAmountText;
    
    private List<BuffData> _canAddStackBuffs = new List<BuffData>();
    private int _rerollAmount = 3;

    private void OnEnable()
    {
        GameEventManager.onAddRerollAmount += UpdateRerollAmount;
    }

    private void OnDisable()
    {
        GameEventManager.onAddRerollAmount -= UpdateRerollAmount;
    }

    private void InitBuffsList()
    {
        var allBuffs = Resources.LoadAll<BuffData>("BuffData");
        _canAddStackBuffs = new List<BuffData>();

        foreach (var buff in allBuffs)
        {
            if(GameplayManager.Instance.buffController.IsBuffMaxStack(buff.type)) continue;
            
            _canAddStackBuffs.Add(buff);
        }
        
        _canAddStackBuffs.Shuffle();
    }

    public override void Show()
    {
        InitBuffsList();
        SetupChooseOption();
        
        base.Show();
    }

    public override void Hide()
    {
        base.Hide();
        
        GameEventManager.InvokeNextWave();
    }

    private void SetupChooseOption()
    {
        for (int i = 0; i < optionButtons.Count; i++)
        {
            if (i >= _canAddStackBuffs.Count)
            {
                optionButtons[i].image.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                optionButtons[i].image.transform.parent.gameObject.SetActive(true);
                optionButtons[i].Setup(_canAddStackBuffs[i]);
            }
        }
    }

    private void UpdateRerollAmount(int amount)
    {
        _rerollAmount += amount;
        rerollAmountText.text = $"Roll: {_rerollAmount}";
    }
    
    public void ChooseOption1()
    {
        Debug.Log("ChooseOption1");
        
        Hide();
    }
    
    public void ChooseOption2()
    {
        Debug.Log("ChooseOption2");
        
        Hide();
    }
    
    public void ChooseOption3()
    {
        Debug.Log("ChooseOption3");
        
        Hide();
    }

    public void Reroll()
    {
        if (_rerollAmount <= 0) return;
        
        _canAddStackBuffs.Shuffle();
        SetupChooseOption();

        UpdateRerollAmount(-1);
    }
}

[Serializable]
public class ChooseOptionButton
{
    public Image image;
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;

    public void Setup(BuffData data)
    {
        image.sprite = data.buffIcon;
        title.text = data.buffName;
        description.text = data.description;
    }
}
