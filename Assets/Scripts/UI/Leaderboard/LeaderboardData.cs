using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class PlayHistory
{
    public string _id;
    public int wave;
    public int score;
    public int playtime;
    public string playedAt;
}

[Serializable]
public class PlayerData
{
    public string _id;
    public string name;
    public List<PlayHistory> history;
    public string createdAt;
    public string __v;

    public PlayHistory GetBestHistory
    {
        get
        {
            if (history == null || history.Count == 0)
            {
                return new PlayHistory 
                { 
                    score = 0, 
                    wave = 0, 
                    playtime = 0, 
                    playedAt = DateTime.Now.ToString("dd-MM-yyyy") 
                };
            }

            return history.OrderByDescending(h => h.score).First();
        }
    }
}


public static class JsonHelper
{
    public static List<T> FromJson<T>(string json)
    {
        try
        {
            string newJson = "{ \"array\": " + json + "}";
            Wrapper<T> wrapper = UnityEngine.JsonUtility.FromJson<Wrapper<T>>(newJson);
            
            if (wrapper == null || wrapper.array == null)
            {
                Debug.LogError("Wrapper or array is null!");
                return new List<T>();
            }
            
            return wrapper.array;
        }
        catch (Exception e)
        {
            Debug.LogError($"JsonHelper.FromJson error: {e.Message}");
            return new List<T>();
        }
    }

    [Serializable]
    private class Wrapper<T>
    {
        public List<T> array;
    }
}