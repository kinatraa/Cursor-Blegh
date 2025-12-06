using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioLibrary audioLibrary;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;
    
    private Dictionary<string, AudioClip> _audioDict = new Dictionary<string, AudioClip>();
    
    protected override void Awake()
    {
        foreach (var e in audioLibrary.sfx)
            _audioDict[e.key] = e.clip;
        
        foreach (var e in audioLibrary.bgm)
            _audioDict[e.key] = e.clip;
    }
    
    public void ShotSfx(string key, float volume = 1f, float pitch = 1f)
    {
        if (_audioDict.TryGetValue(key, out var clip))
        {
            sfxSource.pitch = pitch;
            sfxSource.PlayOneShot(clip, volume);
        }
        else
        {
            Debug.LogWarning($"Audio key '{key}' not found!");
        }
    }

    public void PlayMusic(string key, float volume = 1f, float pitch = 1f)
    {
        if (_audioDict.TryGetValue(key, out var clip))
        {
            musicSource.pitch = pitch;
            musicSource.volume = volume;
            musicSource.clip = clip;
            musicSource.Play();
        }
    }
}


