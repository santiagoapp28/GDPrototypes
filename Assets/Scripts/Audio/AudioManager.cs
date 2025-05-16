using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [System.Serializable]
    public class SoundEntry
    {
        public Sounds soundType;
        public AudioClip clip;
    }

    [System.Serializable]
    public class MusicEntry
    {
        public Music musicType;
        public AudioClip clip;
    }

    [Header("SFX Settings")]
    public SoundEntry[] sfxClips;
    public int sfxSourcePoolSize = 10;

    [Header("Music Settings")]
    public MusicEntry[] musicClips;
    public AudioSource musicSource;

    private Dictionary<Sounds, AudioClip> sfxDict;
    private Dictionary<Music, AudioClip> musicDict;
    private List<AudioSource> sfxSources;

    public bool startInMenu;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeSFX();
        InitializeMusic();

        if(startInMenu)
        {
            PlayMusic(Music.MenuMusic);
        }
        else
        {
            PlayMusic(Music.GameplayMusic);
        }
    }

    void InitializeSFX()
    {
        sfxDict = new Dictionary<Sounds, AudioClip>();
        sfxSources = new List<AudioSource>();

        foreach (var entry in sfxClips)
        {
            if (!sfxDict.ContainsKey(entry.soundType))
                sfxDict.Add(entry.soundType, entry.clip);
        }

        // Create pooled AudioSources for SFX
        for (int i = 0; i < sfxSourcePoolSize; i++)
        {
            AudioSource src = gameObject.AddComponent<AudioSource>();
            src.playOnAwake = false;
            sfxSources.Add(src);
        }
    }

    void InitializeMusic()
    {
        musicDict = new Dictionary<Music, AudioClip>();

        foreach (var entry in musicClips)
        {
            if (!musicDict.ContainsKey(entry.musicType))
                musicDict.Add(entry.musicType, entry.clip);
        }

        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }
    }

    public void PlaySFX(Sounds sound)
    {
        if (sfxDict.TryGetValue(sound, out AudioClip clip))
        {
            AudioSource source = GetAvailableSFXSource();
            if (source != null)
            {
                source.clip = clip;
                source.Play();
            }
        }
    }

    public void PlayMusic(Music music)
    {
        if (musicDict.TryGetValue(music, out AudioClip clip))
        {
            if (musicSource.clip != clip)
            {
                musicSource.clip = clip;
                musicSource.Play();
            }
        }
    }

    AudioSource GetAvailableSFXSource()
    {
        foreach (var src in sfxSources)
        {
            if (!src.isPlaying)
                return src;
        }

        // All sources busy? Override the first one
        return sfxSources[0];
    }
}
public enum Sounds
{
    FireBullet,
    BulletHit,
    EnemyDeath,
    BuyItem,
    UIClick,
    UIHover,
    PlaceSegment,
    StartGame,
    WaveCleared
}

public enum Music
{
    MenuMusic,
    ShopMusic,
    GameplayMusic
}