using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "AudioDatabase",
    menuName = "SO/Audio/Audio Database")]
public class AudioDatabase : ScriptableObject
{
    [SerializeField] private List<AudioData> _audios = new();

    private Dictionary<string, AudioData> _audioDictionary;

    public void Initialize()
    {
        _audioDictionary = new Dictionary<string, AudioData>();

        foreach (AudioData audio in _audios)
        {
            if (audio == null || string.IsNullOrWhiteSpace(audio.Id))
                continue;

            if (_audioDictionary.ContainsKey(audio.Id))
            {
                Debug.LogWarning($"Audio ID bị trùng: {audio.Id}");
                continue;
            }

            _audioDictionary.Add(audio.Id, audio);
        }
    }

    public bool TryGetAudio(string id, out AudioData audio)
    {
        if (_audioDictionary == null)
            Initialize();

        return _audioDictionary.TryGetValue(id, out audio);
    }
}