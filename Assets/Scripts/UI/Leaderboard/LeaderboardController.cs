using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class LeaderboardController : MonoBehaviour
{
    [Header("Settings")]
    private const string BASE_URL = "https://game-backend-wheat.vercel.app";

    [Header("UI References")]
    public Transform contentContainer;
    public GameObject rowPrefab;
    public GameObject loadingIndicator;

    private List<PlayerData> _leaderboardData = new List<PlayerData>();
    
    // get leaderboard data
    public List<PlayerData> GetLeaderboardData() =>
        _leaderboardData
            .OrderByDescending(p => p.GetBestHistory.score)
            .ToList();
    
    public string CurrentPlayerId { get; private set; }
    public string CurrentPlayerName { get; private set; }

    private void Awake()
    {
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
                _leaderboardData = JsonHelper.FromJson<PlayerData>(jsonResult);
            }
        }

        if (loadingIndicator) loadingIndicator.SetActive(false);
    }
    
    // register player
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
                if (request.responseCode == 409)
                {
                    Debug.LogWarning("Tên người chơi đã tồn tại. Sẽ sử dụng tên này để chơi.");
                    CurrentPlayerName = name;
                    PlayerPrefs.SetString("PlayerName", CurrentPlayerName);
                    PlayerPrefs.Save();
                    onComplete?.Invoke(true);
                }
                else
                {
                    Debug.LogError($"Đăng ký thất bại: {request.error}\n{request.downloadHandler.text}");
                    onComplete?.Invoke(false);
                }
            }
            else
            {
                PlayerData newPlayer = JsonUtility.FromJson<PlayerData>(request.downloadHandler.text);
                
                CurrentPlayerId = newPlayer._id;
                CurrentPlayerName = newPlayer.name;
                
                PlayerPrefs.SetString("PlayerID", CurrentPlayerId); 
                PlayerPrefs.SetString("PlayerName", CurrentPlayerName); 
                PlayerPrefs.Save();

                Debug.Log($"Đăng ký thành công! Name: {CurrentPlayerName}");
                onComplete?.Invoke(true);
            }
        }
    }
    
    // update score
    public void SubmitScore(int wave, int score, int playtime)
    {
        if (string.IsNullOrEmpty(CurrentPlayerName))
        {
            Debug.LogError("Chưa có Tên người chơi (PlayerName). Không thể gửi điểm.");
            return;
        }
        StartCoroutine(UpdateScoreRoutine(CurrentPlayerName, wave, score, playtime));
    }

    private IEnumerator UpdateScoreRoutine(string name, int wave, int score, int playtime)
    {
        // "Gamer VN" -> "Gamer%20VN"
        string safeName = Uri.EscapeDataString(name);
        string url = $"{BASE_URL}/update_score/{safeName}";
        
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
                Debug.LogError($"Cập nhật điểm thất bại: {request.error} - {request.downloadHandler.text}");
            }
            else
            {
                Debug.Log($"Cập nhật điểm thành công cho user: {name}!");
                RefreshLeaderboard();
            }
        }
    }
    
    public class DisplayInfo
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