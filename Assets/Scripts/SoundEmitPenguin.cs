using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FisherPenguinSoundManager : MonoBehaviour
{
    public AudioClip[] fishingSounds; // Assign Fishing Hum v1, v2, v3 in the Inspector
    public GameObject player; // Assign the Player GameObject in the Inspector
    public float minFishingInterval = 3f;
    public float maxFishingInterval = 6f;

    private AudioSource audioSource;
    private float nextPlayTime;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1.0f; // Set audio to 3D
        ScheduleNextSound();
    }

    void Update()
    {
        if (Time.time >= nextPlayTime)
        {
            PlayRandomFishingSound();
            ScheduleNextSound();
        }

        AdjustVolumeBasedOnDistance();
    }

    private void PlayRandomFishingSound()
    {
        if (fishingSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, fishingSounds.Length);
            audioSource.clip = fishingSounds[randomIndex];
            audioSource.Play();
        }
    }

    private void ScheduleNextSound()
    {
        nextPlayTime = Time.time + Random.Range(minFishingInterval, maxFishingInterval);
    }

    private void AdjustVolumeBasedOnDistance()
    {
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            float maxDistance = audioSource.maxDistance; // Set this in the AudioSource component
            audioSource.volume = Mathf.Clamp01(1 - (distance / maxDistance));
        }
    }
}