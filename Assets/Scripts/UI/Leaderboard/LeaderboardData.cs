using System;
using System.Collections.Generic;

[Serializable]
public class PlayHistory
{
    public int score;
    public int wave;
    public int playtime;
    public string playedAt;
}

[Serializable]
public class PlayerData
{
    public string _id;
    public string name;
    public List<PlayHistory> history;
}

public static class JsonHelper
{
    public static List<T> FromJson<T>(string json)
    {
        string newJson = "{ \"array\": " + json + "}";
        Wrapper<T> wrapper = UnityEngine.JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    [Serializable]
    private class Wrapper<T>
    {
        public List<T> array;
    }
}