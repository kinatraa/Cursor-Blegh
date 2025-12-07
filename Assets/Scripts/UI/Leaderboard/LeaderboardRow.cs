using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardRow : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI timeText;

    public void UpdateText(PlayerData playerData)
    {
        nameText.text = $"{playerData.name}";
        scoreText.text = $"{playerData.GetBestHistory.score}";
        waveText.text = $"{playerData.GetBestHistory.wave}";
        timeText.text = $"{playerData.GetBestHistory.playtime}";
    }
}
