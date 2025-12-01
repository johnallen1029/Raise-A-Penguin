// 11/4/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

// 11/3/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class FisherPenguin : MonoBehaviour
{
    public int maxFishCount = 100; // Maximum fish the FisherPenguin can hold
    public float minFishingInterval = 2f; // Minimum time between fishing actions
    public float maxFishingInterval = 5f; // Maximum time between fishing actions
    public GameObject player; // Reference to the player object
    public TextMeshProUGUI promptText; // UI element to display the interaction prompt
    public string interactionKey = "E"; // Key to interact with the FisherPenguin
    public TextMeshPro fishCountText; // Billboarded text element to display fish count

    private int fishCount = 0; // Current fish count of the FisherPenguin
    private bool isPlayerNearby = false; // Tracks if the player is near the FisherPenguin
    private bool isFishing = false; // Tracks if the FisherPenguin is actively fishing

    private void Start()
    {
        StartFishing();
        UpdateFishCountText();
    }

    private void Update()
    {
        if (isPlayerNearby)
        {
            // Check if the interaction key is pressed using the New Input System
            Key keyEnum;
            if (System.Enum.TryParse<Key>(interactionKey.ToUpper(), out keyEnum))
            {
                var keyControl = Keyboard.current[keyEnum];
                if (keyControl != null && keyControl.wasPressedThisFrame)
                {
                    CollectFish();
                }
            }
        }

        // Make the fish count text always face the camera
        if (fishCountText != null)
        {
            fishCountText.transform.LookAt(Camera.main.transform);
            fishCountText.transform.Rotate(0, 180, 0); // Rotate to face the camera correctly
        }
    }

    private void StartFishing()
    {
        if (!isFishing)
        {
            isFishing = true;
            StartCoroutine(FishingRoutine());
        }
    }

    private IEnumerator FishingRoutine()
    {
        while (isFishing)
        {
            // Wait for a random time interval between minFishingInterval and maxFishingInterval
            float waitTime = Random.Range(minFishingInterval, maxFishingInterval);
            yield return new WaitForSeconds(waitTime);

            // Add a fish to the FisherPenguin's count if it's below the maximum
            if (fishCount < maxFishCount)
            {
                fishCount++;
                Debug.Log($"FisherPenguin caught a fish! Current fish count: {fishCount}");
                UpdateFishCountText();
            }
        }
    }

    private void CollectFish()
    {
        if (fishCount > 0)
        {
            // Add FisherPenguin's fish count to the player's fish count
            GameManager.Instance.AddFish(fishCount);
            Debug.Log($"Player collected {fishCount} fish from FisherPenguin!");

            // Reset FisherPenguin's fish count
            fishCount = 0;
            UpdateFishCountText();

            // Update the prompt text
            ShowPrompt(false);
        }
        else
        {
            Debug.Log("No fish to collect!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player enters the trigger zone
        if (other.gameObject == player)
        {
            isPlayerNearby = true;
            ShowPrompt(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the player exits the trigger zone
        if (other.gameObject == player)
        {
            isPlayerNearby = false;
            ShowPrompt(false);
        }
    }

    private void ShowPrompt(bool show)
    {
        if (promptText != null)
        {
            promptText.gameObject.SetActive(show);
            if (show)
            {
                promptText.text = fishCount > 0
                    ? $"Press {interactionKey} to collect {fishCount} fish"
                    : "No fish to collect!";
            }
        }
    }

    private void UpdateFishCountText()
    {
        if (fishCountText != null)
        {
            fishCountText.text = $"Fish: {fishCount}";
        }
    }

    public void StopFishing()
    {
        isFishing = false;
    }
}