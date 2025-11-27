using System.Collections.Generic;
using UnityEngine;

public class MonsterSizeConfig : MonoBehaviour
{
    [System.Serializable]
    public class SizeEntry
    {
        public string type;     
        public float scale;     
        public float radius;    
    }

    [System.Serializable]
    public class SizeMultiplier
    {
        public float SMALL;
        public float MEDIUM;
        public float LARGE;
    }

    [System.Serializable]
    public class SizeCollection
    {
        public List<SizeEntry> monster_sizes;
        public SizeMultiplier size_multipliers;
    }

    [Header("Config")]
    public TextAsset jsonFile;

    private Dictionary<MonsterType, SizeEntry> _sizeDict;
    private SizeMultiplier _multipliers;
    
    public static MonsterSizeConfig Instance;

    void Awake()
    {
        Instance = this;
        LoadConfig();
    }

    void LoadConfig()
    {
        if (jsonFile == null) return;

        var data = JsonUtility.FromJson<SizeCollection>(jsonFile.text);
        _sizeDict = new Dictionary<MonsterType, SizeEntry>();
        _multipliers = data.size_multipliers;

        foreach (var item in data.monster_sizes)
        {
            if (System.Enum.TryParse(item.type, out MonsterType mType))
            {
                if (!_sizeDict.ContainsKey(mType))
                {
                    _sizeDict.Add(mType, item);
                }
            }
        }
    }
    
    public float GetRadius(MonsterType type, MonsterSize variantSize)
    {
        if (!_sizeDict.ContainsKey(type)) return 0.5f;

        SizeEntry baseStats = _sizeDict[type];
    
        float multiplier = 1.0f;
        switch (variantSize)
        {
            case MonsterSize.SMALL: multiplier = _multipliers.SMALL; break;
            case MonsterSize.MEDIUM: multiplier = _multipliers.MEDIUM; break;
            case MonsterSize.LARGE: multiplier = _multipliers.LARGE; break;
        }

        return baseStats.radius * multiplier;
    }
    
    public void ApplySize(GameObject monsterObj, MonsterType type, MonsterSize variantSize = MonsterSize.MEDIUM)
    {
        if (!_sizeDict.ContainsKey(type)) return;

        SizeEntry baseStats = _sizeDict[type];
        
        float multiplier = 1.0f;
        switch (variantSize)
        {
            case MonsterSize.SMALL: multiplier = _multipliers.SMALL; break;
            case MonsterSize.MEDIUM: multiplier = _multipliers.MEDIUM; break;
            case MonsterSize.LARGE: multiplier = _multipliers.LARGE; break;
        }

        float finalScale = baseStats.scale * multiplier;
        float finalRadius = baseStats.radius * multiplier;
        
        monsterObj.transform.localScale = Vector3.one * finalScale;
        
        var capsule = monsterObj.GetComponent<CapsuleCollider>();
        if (capsule != null)
        {
            capsule.radius = finalRadius;
            capsule.height = finalRadius * 4;
        }
        
        var agent = monsterObj.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            agent.radius = finalRadius;
        }
    }
}