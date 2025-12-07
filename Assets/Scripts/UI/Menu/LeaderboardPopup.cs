using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardPopup : UIPopup
{
    public LeaderboardRow rowPrefab;
    public LeaderboardController controller;

    public RectTransform container;
    public List<LeaderboardRow> rows = new List<LeaderboardRow>();
    private float _rowHeight = 0.8f;
    private float _rowSpacing = 0;


    public override void Show()
    {
        base.Show();

        var playerList = controller.GetLeaderboardData();
        int count = playerList.Count;

        EnsureRows(count);

        for (int i = 0; i < rows.Count; i++)
        {
            if (i < count)
            {
                rows[i].UpdateText(playerList[i]);
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

            rt.anchoredPosition = new Vector2(0, y - 0.4f);
        }
    }

    public void Exit()
    {
        Hide();
    }
}