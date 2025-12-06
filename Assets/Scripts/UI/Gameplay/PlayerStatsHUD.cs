using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStatsHUD : UIHud
{
    [Header("Health Bar")]
    public List<RectTransform> heartIcons = new List<RectTransform>();

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI waveText;
    
    public void SetupHealthBar(int maxHp)
    {
        for (int i = 0; i < heartIcons.Count; i++)
        {
            if (i < maxHp)
            {
                heartIcons[i].gameObject.SetActive(true);
                heartIcons[i].transform.GetChild(0).gameObject.SetActive(true);
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
                heartIcons[i].transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                heartIcons[i].transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    public void UpdateScoreText(int score)
    {
        scoreText.text = $"Score {score}";
    }

    public void UpdateWaveText(int wave)
    {
        waveText.text = $"Wave {wave}";
    }
}
