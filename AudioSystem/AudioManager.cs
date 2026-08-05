using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private const string MasterVolumeKey = "Audio_MasterVolume";
    private const string MusicVolumeKey = "Audio_MusicVolume";
    private const string SfxVolumeKey = "Audio_SfxVolume";

    [Header("Database")]
    [SerializeField] private AudioDatabase _database;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer _audioMixer;

    [Header("Mixer Parameters")]
    [SerializeField] private string _masterParameter = "MasterVolume";
    [SerializeField] private string _musicParameter = "MusicVolume";
    [SerializeField] private string _sfxParameter = "SFXVolume";

    [Header("Audio Sources")]
    [SerializeField] private AudioSource _musicSource;

    [Header("SFX Pool")]
    [SerializeField] private AudioSource _sfxSourcePrefab;
    [SerializeField] private int _initialPoolSize = 10;

    private readonly Queue<AudioSource> _availableSources = new();
    private readonly List<AudioSource> _activeSources = new();

    private readonly Dictionary<string, int> _playingCount = new();

    private string _currentMusicId;

    private void Awake()
    {
        InitializeSingleton();
        InitializeDatabase();
        InitializePool();
        LoadSettings();
    }

    private void InitializeSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void InitializeDatabase()
    {
        if (_database == null)
        {
            Debug.LogError("AudioDatabase chưa được gán.");
            return;
        }

        _database.Initialize();
    }

    private void InitializePool()
    {
        if (_sfxSourcePrefab == null)
        {
            Debug.LogError("SFX Source Prefab chưa được gán.");
            return;
        }

        for (int i = 0; i < _initialPoolSize; i++)
        {
            CreatePooledSource();
        }
    }

    private AudioSource CreatePooledSource()
    {
        AudioSource source = Instantiate(_sfxSourcePrefab, transform);
        source.gameObject.SetActive(false);

        _availableSources.Enqueue(source);

        return source;
    }

    private void Update()
    {
        ReleaseFinishedSources();
    }

    private void ReleaseFinishedSources()
    {
        for (int i = _activeSources.Count - 1; i >= 0; i--)
        {
            AudioSource source = _activeSources[i];

            if (source.isPlaying)
                continue;

            string audioId = source.gameObject.name;

            if (_playingCount.TryGetValue(audioId, out int count))
            {
                count--;

                if (count <= 0)
                    _playingCount.Remove(audioId);
                else
                    _playingCount[audioId] = count;
            }

            source.clip = null;
            source.transform.SetParent(transform);
            source.transform.localPosition = Vector3.zero;
            source.gameObject.SetActive(false);

            _activeSources.RemoveAt(i);
            _availableSources.Enqueue(source);
        }
    }

    public void PlayMusic(string id, bool restart = false)
    {
        if (!TryGetAudio(id, out AudioData audio))
            return;

        if (audio.Category != AudioCategory.Music)
        {
            Debug.LogWarning($"{id} không thuộc loại Music.");
            return;
        }

        if (_currentMusicId == id && _musicSource.isPlaying && !restart)
            return;

        _currentMusicId = id;

        _musicSource.Stop();
        _musicSource.clip = audio.Clip;
        _musicSource.volume = audio.Volume;
        _musicSource.pitch = audio.Pitch;
        _musicSource.loop = true;
        _musicSource.Play();
    }

    public void StopMusic()
    {
        _musicSource.Stop();
        _currentMusicId = null;
    }

    public void PauseMusic()
    {
        _musicSource.Pause();
    }

    public void ResumeMusic()
    {
        _musicSource.UnPause();
    }

    public AudioSource PlaySFX(string id)
    {
        return PlaySFX(id, Vector3.zero, false);
    }

    public AudioSource PlaySFXAtPosition(string id, Vector3 position)
    {
        return PlaySFX(id, position, true);
    }

    private AudioSource PlaySFX(
        string id,
        Vector3 position,
        bool useWorldPosition)
    {
        if (!TryGetAudio(id, out AudioData audio))
            return null;

        if (audio.Category == AudioCategory.Music)
        {
            Debug.LogWarning($"{id} là Music, không nên phát bằng PlaySFX.");
            return null;
        }

        int currentPlaying = _playingCount.GetValueOrDefault(id);

        if (currentPlaying >= audio.MaxSimultaneous)
            return null;

        AudioSource source = GetAvailableSource();

        source.gameObject.name = id;
        source.gameObject.SetActive(true);

        source.clip = audio.Clip;
        source.volume = audio.Volume;
        source.pitch = audio.Pitch;
        source.loop = audio.Loop;

        if (useWorldPosition)
        {
            source.transform.SetParent(null);
            source.transform.position = position;
            source.spatialBlend = 1f;
        }
        else
        {
            source.transform.SetParent(transform);
            source.transform.localPosition = Vector3.zero;
            source.spatialBlend = 0f;
        }

        source.Play();

        _activeSources.Add(source);
        _playingCount[id] = currentPlaying + 1;

        return source;
    }

    private AudioSource GetAvailableSource()
    {
        if (_availableSources.Count == 0)
            CreatePooledSource();

        return _availableSources.Dequeue();
    }

    public void StopSFX(AudioSource source)
    {
        if (source == null)
            return;

        source.Stop();
    }

    public void StopAllSFX()
    {
        foreach (AudioSource source in _activeSources)
        {
            source.Stop();
        }
    }

    private bool TryGetAudio(string id, out AudioData audio)
    {
        audio = null;

        if (_database == null)
            return false;

        if (_database.TryGetAudio(id, out audio))
            return true;

        Debug.LogWarning($"Không tìm thấy Audio ID: {id}");
        return false;
    }

    public void SetMasterVolume(float value)
    {
        value = Mathf.Clamp01(value);

        SetMixerVolume(_masterParameter, value);
        PlayerPrefs.SetFloat(MasterVolumeKey, value);
    }

    public void SetMusicVolume(float value)
    {
        value = Mathf.Clamp01(value);

        SetMixerVolume(_musicParameter, value);
        PlayerPrefs.SetFloat(MusicVolumeKey, value);
    }

    public void SetSFXVolume(float value)
    {
        value = Mathf.Clamp01(value);

        SetMixerVolume(_sfxParameter, value);
        PlayerPrefs.SetFloat(SfxVolumeKey, value);
    }

    private void SetMixerVolume(string parameter, float value)
    {
        if (_audioMixer == null)
            return;

        float decibel = value <= 0.0001f
            ? -80f
            : Mathf.Log10(value) * 20f;

        _audioMixer.SetFloat(parameter, decibel);
    }

    private void LoadSettings()
    {
        float masterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
        float musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        float sfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, 1f);

        SetMasterVolume(masterVolume);
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
    }
}