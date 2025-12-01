using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class SoundEffectManager : MonoBehaviour
{
    public static SoundEffectManager Instance;
    
    [Header("Audio Mixer")]
    public AudioMixerGroup sfxMixerGroup;
    
    [Header("Fishing Minigame Sounds")]
    public AudioClip keyHitSound;
    public AudioClip successSound;
    public AudioClip failureSound;
    public AudioClip fishCaughtSound;
    public AudioClip punchSound; 
    public AudioClip moneyCollectSound; 
    public AudioClip jumpSound; 
    
    [Header("Audio Source Pool")]
    public int audioSourcePoolSize = 8;
    
    [Header("Specific Mixer Groups")]
    public AudioMixerGroup jumpMixer; 
    public AudioMixerGroup moneyMixer; 
    public AudioMixerGroup keyHitMixer; 

    public AudioMixerGroup punchMixer; 
    
    private AudioSource[] audioSourcePool;
    private int currentSourceIndex = 0;
    
    void Awake()
    {
        // Singleton pattern - only one instance in the game
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep between scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        CreateAudioSourcePool();
    }
    
    void CreateAudioSourcePool()
    {
        audioSourcePool = new AudioSource[audioSourcePoolSize];
        
        for (int i = 0; i < audioSourcePoolSize; i++)
        {
            // Create child GameObject for each AudioSource
            GameObject child = new GameObject($"SFX_AudioSource_{i}");
            child.transform.SetParent(transform);
            
            // Add AudioSource component
            AudioSource source = child.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false;
            
            // Start with default SFX mixer group
            if (sfxMixerGroup != null)
            {
                source.outputAudioMixerGroup = sfxMixerGroup;
            }
            
            audioSourcePool[i] = source;
        }
    }
    
    // Get next available AudioSource (round-robin)
    private AudioSource GetAvailableAudioSource()
    {
        AudioSource source = audioSourcePool[currentSourceIndex];
        currentSourceIndex = (currentSourceIndex + 1) % audioSourcePoolSize;
        return source;
    }
    
    // Public methods to play specific sounds WITH MIXER GROUPS
    public void PlayKeyHit(float pitch = 1f)
    {
        if (keyHitMixer != null)
            PlaySoundWithMixerGroup(keyHitSound, keyHitMixer, pitch);
        else
            PlaySound(keyHitSound);
    }
    
    public void PlaySuccess()
    {
        PlaySound(successSound);
    }
    
    public void PlayFailure()
    {
        PlaySound(failureSound);
    }
    
    public void PlayFishCaught()
    {
        PlaySound(fishCaughtSound);
    }

    public void PlayPunchSound()
    {
         if (keyHitMixer != null)
            PlaySoundWithMixerGroup(punchSound, punchMixer);
        else
            PlaySound(punchSound);
    }

    public void PlayMoneySound()
    {
        if (moneyMixer != null)
            PlaySoundWithMixerGroup(moneyCollectSound, moneyMixer);
        else
            PlaySound(moneyCollectSound); 
    }
    
    public void PlayJumpSound()
    {
        float randomMin = 0.8f; // Increased minimum to prevent extreme low pitches
        float randomMax = 1.5f; // Reduced maximum to prevent extreme high pitches
        float randomPitch = Random.Range(randomMin, randomMax); 
        
        if (jumpMixer != null)
            PlaySoundWithMixerGroup(jumpSound, jumpMixer, randomPitch);
        else
            PlaySound(jumpSound, randomPitch); 
    }
    
    // Generic method to play any sound (uses default SFX mixer)
    private void PlaySound(AudioClip clip, float pitch = 1f)
    {
        if (clip != null)
        {
            AudioSource source = GetAvailableAudioSource();
            
            // DON'T store original settings - each AudioSource should be configured fresh
            // Reset to defaults first
            source.pitch = pitch;
            source.outputAudioMixerGroup = sfxMixerGroup;
            
            // Play the sound
            source.PlayOneShot(clip);
            
            // NO COROUTINES - let the AudioSource keep its settings until next use
        }
        else
        {
            Debug.LogWarning("Sound effect clip is missing!");
        }
    }
    
    // Method to play sound with specific mixer group
    public void PlaySoundWithMixerGroup(AudioClip clip, AudioMixerGroup mixerGroup, float pitch = 1f)
    {
        if (clip != null && mixerGroup != null)
        {
            AudioSource source = GetAvailableAudioSource();
            
            // Configure fresh for this sound
            source.outputAudioMixerGroup = mixerGroup;
            source.pitch = pitch;
            
            // Play the sound
            source.PlayOneShot(clip);
            
            // NO COROUTINES - settings persist until this AudioSource is used again
        }
        else
        {
            Debug.LogWarning("Sound effect clip or mixer group is missing!");
        }
    }
    
    // Volume control for specific mixer groups
    public void SetSFXVolume(float volume)
    {
        SetMixerGroupVolume(sfxMixerGroup, volume);
    }
    
    public void SetJumpVolume(float volume)
    {
        SetMixerGroupVolume(jumpMixer, volume);
    }
    
    public void SetMoneyVolume(float volume)
    {
        SetMixerGroupVolume(moneyMixer, volume);
    }
    
    public void SetKeyHitVolume(float volume)
    {
        SetMixerGroupVolume(keyHitMixer, volume);
    }
    
    private void SetMixerGroupVolume(AudioMixerGroup mixerGroup, float volume)
    {
        if (mixerGroup != null && mixerGroup.audioMixer != null)
        {
            // Convert linear 0-1 volume to decibels (-80 to 0)
            float dB = volume > 0.0001f ? 20f * Mathf.Log10(volume) : -80f;
            
            // Use a consistent parameter name
            mixerGroup.audioMixer.SetFloat("SFXVolume", dB);
        }
    }
    
    // Master control for all SFX
    public void SetAllSFXVolume(float volume)
    {
        SetSFXVolume(volume);
        SetJumpVolume(volume);
        SetMoneyVolume(volume);
        SetKeyHitVolume(volume);
    }
    
    // NEW: Method to reset all AudioSources to defaults (call this if issues persist)
    public void ResetAllAudioSources()
    {
        foreach (AudioSource source in audioSourcePool)
        {
            if (source != null)
            {
                source.pitch = 1f;
                source.outputAudioMixerGroup = sfxMixerGroup;
                source.Stop();
            }
        }
    }
}