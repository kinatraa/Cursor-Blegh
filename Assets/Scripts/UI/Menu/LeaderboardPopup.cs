using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardPopup : UIPopup
{
    public LeaderboardRow rowPrefab;

    public RectTransform container;
    public List<LeaderboardRow> rows = new List<LeaderboardRow>();
    private float _rowHeight = 0.7838001f;
    private float _rowSpacing = 0.7838001f;

    
    public override void Show()
    {
        base.Show();

        StartCoroutine(LoadAndDisplayLeaderboard());
    }

    private IEnumerator LoadAndDisplayLeaderboard()
    {
        yield return StartCoroutine(LeaderboardController.Instance.RefreshLeaderboard());
        
        var playerList = LeaderboardController.Instance.GetLeaderboardData();
        int count = playerList.Count;
        Debug.Log($"PLAYER LEADERBOARD: {count}");

        if (count == 0)
        {
            Debug.LogWarning("No players with history to display!");
            yield break;
        }
        
        EnsureRows(count);
        
        for (int i = 0; i < rows.Count; i++)
        {
            if (i < count)
            {
                var player = playerList[i];
                Debug.Log($"Displaying player {i}: {player.name} - Score: {player.GetBestHistory.score}");
                
                rows[i].UpdateText(player);
                rows[i].gameObject.SetActive(true);
            }
            else
            {
                rows[i].gameObject.SetActive(false);
            }
        }
        
        ResizeContainer(count);
        RepositionRows();
    }
    
    private void EnsureRows(int targetCount)
    {
        while (rows.Count < targetCount)
        {
            var row = Instantiate(rowPrefab, container);
            row.transform.localScale = Vector3.one;
            row.gameObject.SetActive(true);
            rows.Add(row);
        }
    }

    private void ResizeContainer(int count)
    {
        float totalHeight = count * _rowHeight + (count - 1) * _rowSpacing;

        var size = container.sizeDelta;
        size.y = totalHeight;
        container.sizeDelta = size;
    }

    private void RepositionRows()
    {
        for (int i = 0; i < rows.Count; i++)
        {
            RectTransform rt = rows[i].GetComponent<RectTransform>();
        
            float y = -i * (_rowHeight + _rowSpacing);

            rt.anchoredPosition = new Vector2(0, y);
        }
    }

    public void Exit()
    {
        Hide();
    }
}
