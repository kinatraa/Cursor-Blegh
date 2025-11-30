using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq; // Cần dùng để sắp xếp List

public class LeaderboardController : MonoBehaviour
{
    [Header("Settings")]
    public string apiUrl = "https://game-backend-wheat.vercel.app/get_all_players";
    
    [Header("UI References")]
    public Transform contentContainer;
    public GameObject rowPrefab;     
    public GameObject loadingIndicator;

    void Start()
    {
        StartCoroutine(FetchLeaderboard());
    }

    IEnumerator FetchLeaderboard()
    {
        if (loadingIndicator) loadingIndicator.SetActive(true);

        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error fetching leaderboard: " + request.error);
            }
            else
            {
                string jsonResult = request.downloadHandler.text;
                List<PlayerData> allPlayers = JsonHelper.FromJson<PlayerData>(jsonResult);

                ProcessData(allPlayers);
            }
        }

        if (loadingIndicator) loadingIndicator.SetActive(false);
    }

    void ProcessData(List<PlayerData> rawData)
    {
        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }

        var processedList = new List<DisplayData>();

        foreach (var player in rawData)
        {
            if (player.history == null || player.history.Count == 0) continue;
            PlayHistory bestRun = player.history.OrderByDescending(h => h.score).First();

            processedList.Add(new DisplayData
            {
                name = string.IsNullOrEmpty(player.name) ? "Unknown" : player.name,
                score = bestRun.score,
                wave = bestRun.wave,
                playedAt = bestRun.playedAt
            });
        }

        processedList = processedList.OrderByDescending(x => x.score).ToList();

        for (int i = 0; i < processedList.Count; i++)
        {
            GameObject newRow = Instantiate(rowPrefab, contentContainer);
            LeaderboardRowUI rowUI = newRow.GetComponent<LeaderboardRowUI>();
            
            if (rowUI != null)
            {
                // Rank là i + 1
                rowUI.SetData(i + 1, processedList[i].name, processedList[i].score, processedList[i].wave, processedList[i].playedAt);
            }
        }
    }

    private class DisplayData
    {
        public string name;
        public int score;
        public int wave;
        public string playedAt;
    }
}