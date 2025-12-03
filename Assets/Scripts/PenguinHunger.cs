using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PenguinHunger : MonoBehaviour
{
    [SerializeField] private Slider hungerSlider;

    private float interactionDistance = 3f;
    [SerializeField] private GameObject player;

    [SerializeField] public FishFeeder fishFeeder;

    [SerializeField] public HealthBar healthBar;

    private Camera mainCamera;

    private float maxHunger = 100f;
    public float hungerDrainRate = 0.5f;

    public float healthDrainRate = 5f;
    private int fishRestoreAmount = 20;

    public float maxHealth = 50;
    public float currentHealth;

    public float currentHunger;

    private bool isHealthDraining = false;

    [Header("Eating Sound Settings")]
    [SerializeField] private AudioSource audioSource; // Assign the AudioSource component
    [SerializeField] private AudioClip[] eatingSounds; // Assign eating sound clips in the Inspector
    private float soundCooldown = 5f; // Cooldown duration in seconds
    private float lastSoundTime = -5f; // Tracks the last time a sound was played

    private void Start()
    {
        currentHunger = maxHunger;
        UpdateHungerUI();
        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth, maxHealth);
        mainCamera = Camera.main;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBar.SetHealth(currentHealth, maxHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        FindFirstObjectByType<GameOverManager>().TriggerGameOver();
    }

    private void Update()
    {
        // Drain
        hungerSlider.transform.LookAt(hungerSlider.transform.position + mainCamera.transform.forward);
        if (!isHealthDraining)
        {
            currentHunger -= hungerDrainRate * Time.deltaTime;
            currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);
            UpdateHungerUI();

            if (currentHunger <= 0)
            {
                OnHungerDepleted();
            }
        }
        else
        {
            currentHealth -= healthDrainRate * Time.deltaTime;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            healthBar.SetHealth(currentHealth, maxHealth);

            if (currentHealth <= 0)
            {
                healthBar.SetHealth(0, maxHealth);
                Die();
            }
        }

        if (Vector3.Distance(player.transform.position, transform.position) <= interactionDistance)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                FeedPenguin();
            }
        }
    }

    public void FeedPenguin(bool fromFeeder = false)
    {
        if (fromFeeder)
        {
            currentHunger += fishRestoreAmount;
            currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);
            Debug.Log("Fish fed from feeder");
        }
        else
        {
            if (GameManager.Instance.PlayerFish > 0)
            {
                GameManager.Instance.AddFish(-1);
                Debug.Log("PenguinFed!");
                PlayEatingSound(); // Play eating sound when fed

                currentHunger += fishRestoreAmount;
                currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);

                UpdateHungerUI();
            }
            else
            {
                Debug.Log("Not enough fish!");
            }
        }

        if (currentHunger > 0)
        {
            isHealthDraining = false;
        }

        
    }

    private void UpdateHungerUI()
    {
        if (hungerSlider != null)
        {
            hungerSlider.value = currentHunger / maxHunger;
        }
    }

    private void OnHungerDepleted()
    {
        Debug.Log("Penguin is starving!!");
        if (fishFeeder.storedFish < 1)
        {
            isHealthDraining = true;
        }
    }

    private void PlayEatingSound()
    {
        if (Time.time >= lastSoundTime + soundCooldown)
        {
            if (eatingSounds.Length > 0)
            {
                int randomIndex = Random.Range(0, eatingSounds.Length);
                audioSource.clip = eatingSounds[randomIndex];
                audioSource.Play();
                lastSoundTime = Time.time; // Update the last sound play time
            }
        }
    }
}