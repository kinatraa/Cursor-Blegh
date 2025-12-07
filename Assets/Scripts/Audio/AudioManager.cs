using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioLibrary audioLibrary;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    private int _sfxPoolSize = 100;
    private float _minPitch = 0.9f;
    private float _maxPitch = 1.1f;
    
    private List<AudioSource> _sfxPool = new List<AudioSource>();
    private Dictionary<string, float> _lastPlayedTime = new Dictionary<string, float>();
    private int _curSourceIndex = 0;
    
    private Dictionary<string, AudioClip> _audioDict = new Dictionary<string, AudioClip>();
    private float _bgmVolumeScale = 0.5f;
    private float _sfxVolumeScale = 0.5f;
    
    protected override void Awake()
    {
        foreach (var e in audioLibrary.sfx)
            _audioDict[e.key] = e.clip;
        
        foreach (var e in audioLibrary.bgm)
            _audioDict[e.key] = e.clip;

        InitAudioPool();
    }

    private void InitAudioPool()
    {
        _sfxPool = new List<AudioSource>();
        
        GameObject container = new GameObject("SFX_Pool_Container");
        container.transform.SetParent(transform);

        for (int i = 1; i < _sfxPoolSize; i++)
        {
            GameObject obj = new GameObject($"SFX_Source_{i}");
            obj.transform.SetParent(container.transform);
            
            AudioSource source = obj.AddComponent<AudioSource>();
            
            source.playOnAwake = false;
            source.loop = false;
            source.spatialBlend = 0f;
            source.priority = 60;
            
            _sfxPool.Add(source);
        }
    }

    private void OnEnable()
    {
        GameEventManager.onChangeBgmVolume += ChangeBgmVolume;
        GameEventManager.onChangeSfxVolume += ChangeSfxVolume;
    }

    private void OnDisable()
    {
        GameEventManager.onChangeBgmVolume -= ChangeBgmVolume;
        GameEventManager.onChangeSfxVolume -= ChangeSfxVolume;
    }

    private void ChangeBgmVolume(int newVolume)
    {
        _bgmVolumeScale = newVolume / 100f;
        musicSource.volume = _bgmVolumeScale;
    }

    private void ChangeSfxVolume(int newVolume)
    {
        _sfxVolumeScale = newVolume / 100f;
    }

    public void ShotSfx(string key, float volume = 1f, float pitch = 1f)
    {
        if (_audioDict.TryGetValue(key, out var clip))
        {
            _lastPlayedTime[key] = Time.time;

            AudioSource sourceToUse = GetAvailableSource();
            
            sourceToUse.Stop();
            
            sourceToUse.pitch = pitch * UnityEngine.Random.Range(_minPitch, _maxPitch);
            sourceToUse.volume = volume;
            sourceToUse.clip = clip;
            sourceToUse.Play();
        }
        else
        {
            Debug.LogWarning($"Audio key '{key}' not found!");
        }
    }

    private AudioSource GetAvailableSource()
    {
        for (int i = 0; i < _sfxPool.Count; i++)
        {
            if (!_sfxPool[i].isPlaying)
                return _sfxPool[i];
        }
        
        _curSourceIndex++;
        if (_curSourceIndex >= _sfxPool.Count)
        {
            _curSourceIndex = 0;
        };
        
        return _sfxPool[_curSourceIndex];
    }

    public void PlayMusic(string key, float volume = 1f, float pitch = 1f)
    {
        if (_audioDict.TryGetValue(key, out var clip))
        {
            musicSource.pitch = pitch;
            musicSource.volume = volume * _bgmVolumeScale;
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }
}


