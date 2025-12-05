using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class LeaderboardController : MonoBehaviour
{
    public static LeaderboardController Instance { get; private set; }

    [Header("Settings")]
    private const string BASE_URL = "https://game-backend-wheat.vercel.app";

    [Header("UI References")]
    public Transform contentContainer;
    public GameObject rowPrefab;
    public GameObject loadingIndicator;
    
    public string CurrentPlayerId { get; private set; }
    public string CurrentPlayerName { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        CurrentPlayerId = PlayerPrefs.GetString("PlayerID", "");
        CurrentPlayerName = PlayerPrefs.GetString("PlayerName", "");
    }

    private void Start()
    {
        if (contentContainer != null)
        {
            RefreshLeaderboard();
        }
    }
    
    public void RefreshLeaderboard()
    {
        StartCoroutine(FetchLeaderboardRoutine());
    }

    private IEnumerator FetchLeaderboardRoutine()
    {
        if (loadingIndicator) loadingIndicator.SetActive(true);

        string url = $"{BASE_URL}/get_all_players";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Lỗi lấy BXH: {request.error}");
            }
            else
            {
                string jsonResult = request.downloadHandler.text;
                
                List<PlayerData> allPlayers = JsonHelper.FromJson<PlayerData>(jsonResult);
                
                ProcessAndDisplayData(allPlayers);
            }
        }

        if (loadingIndicator) loadingIndicator.SetActive(false);
    }

    private void ProcessAndDisplayData(List<PlayerData> rawData)
    {
        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }
        
        var displayList = new List<DisplayInfo>();

        foreach (var player in rawData)
        {
            if (player.history == null || player.history.Count == 0) continue;
            
            PlayHistory bestRun = player.history.OrderByDescending(h => h.score).First();

            displayList.Add(new DisplayInfo
            {
                name = string.IsNullOrEmpty(player.name) ? "Unknown" : player.name,
                score = bestRun.score,
                wave = bestRun.wave,
                date = bestRun.playedAt
            });
        }
        
        displayList = displayList.OrderByDescending(x => x.score).ToList();
        
        for (int i = 0; i < displayList.Count; i++)
        {
            GameObject newRow = Instantiate(rowPrefab, contentContainer);
            LeaderboardRowUI rowUI = newRow.GetComponent<LeaderboardRowUI>();

            if (rowUI != null)
            {
                rowUI.SetData(
                    i + 1, 
                    displayList[i].name, 
                    displayList[i].score, 
                    displayList[i].wave, 
                    displayList[i].date
                );
            }
        }
    }
    
    public void RegisterPlayer(string playerName, Action<bool> onComplete = null)
    {
        StartCoroutine(RegisterPlayerRoutine(playerName, onComplete));
    }

    private IEnumerator RegisterPlayerRoutine(string name, Action<bool> onComplete)
    {
        string url = $"{BASE_URL}/add_player";
        
        CreatePlayerRequest requestBody = new CreatePlayerRequest { name = name };
        string jsonData = JsonUtility.ToJson(requestBody);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Đăng ký thất bại: {request.error}\n{request.downloadHandler.text}");
                onComplete?.Invoke(false);
            }
            else
            {
                PlayerData newPlayer = JsonUtility.FromJson<PlayerData>(request.downloadHandler.text);
                
                CurrentPlayerId = newPlayer._id;
                CurrentPlayerName = newPlayer.name;
                
                PlayerPrefs.SetString("PlayerID", CurrentPlayerId);
                PlayerPrefs.SetString("PlayerName", CurrentPlayerName);
                PlayerPrefs.Save();

                Debug.Log($"Đăng ký thành công! ID: {CurrentPlayerId}");
                onComplete?.Invoke(true);
            }
        }
    }
    
    public void SubmitScore(int wave, int score, int playtime)
    {
        if (string.IsNullOrEmpty(CurrentPlayerId))
        {
            Debug.LogError("Chưa có PlayerID. Không thể gửi điểm.");
            return;
        }
        StartCoroutine(UpdateScoreRoutine(CurrentPlayerId, wave, score, playtime));
    }

    private IEnumerator UpdateScoreRoutine(string id, int wave, int score, int playtime)
    {
        string url = $"{BASE_URL}/update_score/{id}";
        
        UpdateScoreRequest requestBody = new UpdateScoreRequest 
        { 
            wave = wave, 
            score = score, 
            playtime = playtime 
        };
        string jsonData = JsonUtility.ToJson(requestBody);
        
        using (UnityWebRequest request = new UnityWebRequest(url, "PATCH"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Cập nhật điểm thất bại: {request.error}");
            }
            else
            {
                Debug.Log("Cập nhật điểm thành công lên Server!");
                RefreshLeaderboard();
            }
        }
    }
    private class DisplayInfo
    {
        public string name;
        public int score;
        public int wave;
        public string date;
    }
    [Serializable]
    private class CreatePlayerRequest
    {
        public string name;
    }
    [Serializable]
    private class UpdateScoreRequest
    {
        public int wave;
        public int score;
        public int playtime;
    }
}