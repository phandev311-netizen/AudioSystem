using System;
using UnityEngine;

[Serializable]
public class AudioData
{
    [SerializeField] private string _id;
    [SerializeField] private AudioClip _clip;
    [SerializeField] private AudioCategory _category;

    [Range(0f, 1f)]
    [SerializeField] private float _volume = 1f;

    [Range(0.1f, 3f)]
    [SerializeField] private float _pitch = 1f;

    [SerializeField] private bool _loop;

    [Min(1)]
    [SerializeField] private int _maxSimultaneous = 5;

    public string Id => _id;
    public AudioClip Clip => _clip;
    public AudioCategory Category => _category;
    public float Volume => _volume;
    public float Pitch => _pitch;
    public bool Loop => _loop;
    public int MaxSimultaneous => _maxSimultaneous;
}