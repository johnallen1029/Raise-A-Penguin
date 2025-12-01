using UnityEngine;
using System.Collections.Generic;

public class JumpSoundManager : MonoBehaviour
{
    public static JumpSoundManager Instance;
    
    [Header("Fishing Minigame Sounds")]
    public AudioClip keyHitSound;
    public AudioClip successSound;
    public AudioClip failureSound;
    public AudioClip fishCaughtSound;

    public AudioClip punchSound; 
    
    public AudioClip moneyCollectSound; 
    private AudioSource audioSource;
    public AudioClip jumpSound; 
    
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
        
        // Get or add AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Configure AudioSource for sound effects
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }
    
    // Public methods to play specific sounds
    public void PlayKeyHit()
    {
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
        PlaySound(punchSound); 
    }

    public void PlayMoneySound()
    {
        PlaySound(moneyCollectSound); 
    }
    public void PlayJumpSound()
    {
        float randomMin = 0f; 
        float randomMax = 2f; 

        float randomPitch = Random.Range(randomMin, randomMax); 

        PlaySound(jumpSound, randomPitch); 
    }
    // Generic method to play any sound
    private void PlaySound(AudioClip clip, float pitch = 1)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.pitch = pitch; 
            audioSource.PlayOneShot(clip); // Play without interrupting other sounds
            StartCoroutine(ResetPitch(1, 0.5f)); 
            
        }
        else
        {
            Debug.LogWarning("Sound effect or AudioSource is missing!");
        }
    }
    private System.Collections.IEnumerator ResetPitch(float originalPitch, float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.pitch = originalPitch; 
    }
    
    // Volume control
    public void SetSFXVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = Mathf.Clamp01(volume);
        }
    }
}