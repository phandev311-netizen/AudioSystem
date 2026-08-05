# 🎵 Unity Audio Manager

A lightweight, data-driven Audio Manager for Unity using **ScriptableObject**, **AudioMixer**, and **AudioSource Pooling**.

Designed for small and medium-sized games with clean architecture and easy scalability.

---

## Features

- ✅ ScriptableObject Audio Database
- ✅ Audio ID lookup
- ✅ Background Music (BGM)
- ✅ Sound Effects (SFX)
- ✅ 3D Spatial Audio Support
- ✅ AudioSource Object Pool
- ✅ Max Simultaneous Sound Limit
- ✅ AudioMixer Volume Control
- ✅ PlayerPrefs Volume Saving
- ✅ Singleton Pattern
- ✅ Easy to Extend

---

## Folder Structure

```
Audio
├── AudioData.cs
├── AudioDatabase.cs
├── AudioManager.cs
├── AudioCategory.cs
└── AudioDatabase.asset
```

---

## AudioData

Each sound is stored as an `AudioData`.

```csharp
ID
Clip
Category
Volume
Pitch
Loop
Max Simultaneous
```

Example

| Property | Value |
|----------|-------|
| ID | Button_Click |
| Category | SFX |
| Volume | 1 |
| Pitch | 1 |
| Loop | false |

---

## AudioDatabase

Stores every `AudioData` inside one ScriptableObject.

Features

- Builds Dictionary automatically
- Fast lookup by ID
- Duplicate ID detection
- Ignore invalid entries

Example

```csharp
_database.TryGetAudio("Button_Click", out AudioData audio);
```

---

## AudioManager

Central audio controller.

### Music

```csharp
AudioManager.Instance.PlayMusic("MainTheme");

AudioManager.Instance.StopMusic();

AudioManager.Instance.PauseMusic();

AudioManager.Instance.ResumeMusic();
```

---

### Sound Effect

```csharp
AudioManager.Instance.PlaySFX("Explosion");

AudioManager.Instance.PlaySFX("Button_Click");
```

---

### 3D Sound

```csharp
AudioManager.Instance.PlaySFXAtPosition(
    "Explosion",
    enemy.transform.position
);
```

---

## AudioSource Pool

Instead of creating new AudioSources every time,

AudioManager keeps a pool.

```
Available Queue
      ↓
 Play SFX
      ↓
Active List
      ↓
Sound Finished
      ↓
Return to Pool
```

Benefits

- No runtime Instantiate spam
- Less Garbage Collection
- Better performance

---

## Max Simultaneous

Each sound can define

```
MaxSimultaneous
```

Example

```
Explosion
Max = 5
```

If five explosions are already playing,

new requests will be ignored.

This prevents audio spam.

---

## Volume Control

Supports

- Master
- Music
- SFX

Example

```csharp
AudioManager.Instance.SetMasterVolume(1f);

AudioManager.Instance.SetMusicVolume(0.6f);

AudioManager.Instance.SetSFXVolume(0.8f);
```

Uses

```
AudioMixer
```

and automatically saves

```
PlayerPrefs
```

---

## Singleton

```csharp
AudioManager.Instance
```

Automatically survives scene changes.

```
DontDestroyOnLoad()
```

---

## Usage

### Play Music

```csharp
AudioManager.Instance.PlayMusic("MainTheme");
```

### Play SFX

```csharp
AudioManager.Instance.PlaySFX("Hit");
```

### Play 3D Audio

```csharp
AudioManager.Instance.PlaySFXAtPosition(
    "Explosion",
    transform.position
);
```

### Stop Music

```csharp
AudioManager.Instance.StopMusic();
```

### Pause Music

```csharp
AudioManager.Instance.PauseMusic();
```

### Resume Music

```csharp
AudioManager.Instance.ResumeMusic();
```

---

## Performance

Current implementation includes:

- Dictionary lookup (O(1))
- AudioSource Pooling
- Reusable AudioSources
- Runtime Pool Expansion
- Simultaneous Sound Limiter
- Cached Audio Database

Suitable for mobile and desktop projects.

---

## Requirements

- Unity 2022+
- AudioMixer
- ScriptableObject

---

## Future Improvements

- Audio Fade In / Fade Out
- Crossfade Music
- Audio Groups
- Random Clip Variations
- Footstep Surface System
- Addressables Support
- Async Audio Loading
- Audio Events
- Custom Editor
- Inspector Preview

---

## License

MIT License
