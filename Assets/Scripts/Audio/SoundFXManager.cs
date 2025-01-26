using System.Collections;
using UnityEngine;

public class SoundFXManager : MonoSingleton<SoundFXManager>
{

    [SerializeField] private AudioClip backgroundMusic;
    [Range(0f, 1f)] public float backgroundMusicVolume = 0.7f;

    private AudioSource _musicSource;
    
    
    private void Awake()
    {
        Initialize();
    }

    private void Update()
    {
        _musicSource.volume = backgroundMusicVolume;
    }

    private void Initialize()
    {
        // Initialize the music AudioSource
        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.clip = backgroundMusic;
        _musicSource.loop = true;
        _musicSource.spatialBlend = 0f; // Non-spatial
        _musicSource.volume = backgroundMusicVolume;
        _musicSource.Play();

        // Initialize the AudioSourcePool
        // audioSourcePool = gameObject.AddComponent<AudioSourcePool>();
    }

    public void PlaySpatialSound(AudioClip clip, Transform spawnTransform, float volume = 1.0f)
    {
        var pooledAudio = AudioPool.Instance.Get();
        // AudioSource sourceAudio = pooledAudio.GetComponent<AudioSource>();
        pooledAudio.transform.position = spawnTransform.position;
        // sourceAudio.volume = volume;
        // sourceAudio.clip = clip;
        // sourceAudio.Play();
        pooledAudio.PlaySound(clip, volume);
        StartCoroutine(ReturnToPoolAfterPlaying(pooledAudio, clip.length));
    }
    
    public void ChangeBackgroundMusic(AudioClip newClip)
    {
        _musicSource.clip = newClip;
        _musicSource.Play();
    }
    
    public void ResetBackgroundMusic()
    {
        _musicSource.clip = backgroundMusic;
        _musicSource.Play();
    }

    private IEnumerator ReturnToPoolAfterPlaying(PoolableAudio source, float delay)
    {
        // AudioSource audioSource = source.GetComponent<AudioSource>();
        // yield return new WaitWhile(() => audioSource.isPlaying);
        yield return new WaitForSeconds(delay);
        AudioPool.Instance.Return(source);
    }
}
