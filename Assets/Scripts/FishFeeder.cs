using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishFeeder : MonoBehaviour
{
    public int storedFish = 0;
    private int maxStoredFish = 10;

    private Key interactionKey = Key.E;

    private bool playerInRange = false;

    [SerializeField] private PenguinHunger penguinHunger;

    [SerializeField] private TextMeshPro fishText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        updateFeederText();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && Keyboard.current[interactionKey].wasPressedThisFrame)
        {
            StoreFish();
        }
        if (penguinHunger != null && storedFish > 0 && penguinHunger.currentHunger <= 0)
        {
            FishFeedPenguin();
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true; 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false; 
        }
    }

    public void StoreFish()
    {
        if (GameManager.Instance.PlayerFish > 0 && storedFish < maxStoredFish)
        {
            GameManager.Instance.AddFish(-1);
            storedFish++;
            updateFeederText();
        }
        else if (storedFish >= maxStoredFish)
        {
            Debug.Log("Feeder is full");
        }
        else
        {
            Debug.Log("No fish to store!");
        }
    }


    public void AddFish(int amount)
    {
        storedFish += amount;
        storedFish = Mathf.Clamp(storedFish, 0, maxStoredFish);
    }
    private void FishFeedPenguin()
    {

        if (storedFish > 0)
        {
            storedFish--;
            penguinHunger.FeedPenguin(true);
            updateFeederText();
        }
        else
        {
            Debug.Log("No fish in feeder");
        }
    }
    private void updateFeederText()
    {
        fishText.text = $"Fish: {storedFish}/{maxStoredFish}"; 
    }
}
