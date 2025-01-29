using UnityEngine;

public class PoolableAudio : MonoBehaviour, IPoolable
{
    private AudioSource _audioSource;
    
    private void Awake()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
    }
    
    public void Reset()
    {
        // Reset the audio source
        _audioSource.Stop();
        _audioSource.clip = null;
        _audioSource.volume = 0f;
        _audioSource.loop = false;
        _audioSource.spatialBlend = 0f; // Non-spatial
        _audioSource.pitch = 1f;
    }

    public void Kill()
    {
        AudioPool.Instance.Return(this);
    }

    // Method to play a sound
    public void PlaySound(AudioClip clip, float volume = 1.0f)
    {
        _audioSource.clip = clip;
        _audioSource.volume = volume;
        _audioSource.spatialBlend = 0f; // Fully spatial
        _audioSource.Play();
    }
}
