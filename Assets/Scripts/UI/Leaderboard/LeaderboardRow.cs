using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardRow : MonoBehaviour
{
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI timeText;

    public void UpdateText(PlayerData playerData)
    {
        if (playerData == null)
        {
            Debug.LogError("PlayerData is null!");
            return;
        }

        PlayHistory bestHistory = playerData.GetBestHistory;
        
        if (bestHistory == null)
        {
            Debug.LogError($"BestHistory is null for player: {playerData.name}");
            return;
        }

        if (nameText != null)
            nameText.text = playerData.name ?? "Unknown";
        else
            Debug.LogWarning("nameText is not assigned!");

        if (scoreText != null)
            scoreText.text = bestHistory.score.ToString();
        else
            Debug.LogWarning("scoreText is not assigned!");

        if (waveText != null)
            waveText.text = bestHistory.wave.ToString();
        else
            Debug.LogWarning("waveText is not assigned!");

        if (timeText != null)
            timeText.text = bestHistory.playtime.ToString() ?? "";
        else
            Debug.LogWarning("dateText is not assigned!");
    }
}
