using UnityEngine;
using TMPro; // Nhớ sử dụng TextMeshPro

public class LeaderboardRowUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI dateText; 

    public void SetData(int rank, string name, int score, int wave, string dateStr)
    {
        rankText.text = $"#{rank}";
        nameText.text = name;
        scoreText.text = score.ToString("N0");
        waveText.text = $"Wave {wave}";
        
        if(System.DateTime.TryParse(dateStr, out System.DateTime date))
        {
            dateText.text = date.ToString("dd/MM/yyyy");
        }
        else
        {
            dateText.text = "";
        }

        if (rank == 1) rankText.color = Color.yellow;
        else if (rank == 2) rankText.color = Color.gray; 
        else if (rank == 3) rankText.color = new Color(0.8f, 0.5f, 0.2f); 
        else rankText.color = Color.white;
    }
}