// 11/4/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro; // Import the New Input System namespace

[RequireComponent(typeof(BoxCollider))]
public class FishingInteraction : MonoBehaviour
{
    public GameObject player; // Reference to the player object
    public GameObject upgradeMenu; // Reference to the upgrade menu UI
    public TextMeshProUGUI promptText; // Reference to the UI text for the prompt
    public string interactionKey = "E"; // Key to interact with the computer

    public GameObject fishingTarget; 
    private bool isPlayerNearby = false;

    private void Start()
    {
        // Ensure the BoxCollider is set as a trigger
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            boxCollider.isTrigger = true;
        }
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
                    ShowPrompt(false);
                    StartFishingMinigame(); 
                }
            }
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

    void StartFishingMinigame()
    {
        upgradeMenu.SetActive(true);

        FishingMinigame fishingMinigame = upgradeMenu.GetComponent<FishingMinigame>();
        fishingMinigame.fishingTarget = fishingTarget; 
        fishingMinigame.StartFishing(); 
    }

    void ShowPrompt(bool show)
    {
        if (promptText != null)
        {
            promptText.gameObject.SetActive(show);
            if (show)
            {
                promptText.text = $"Press {interactionKey} to interact";
            }
        }
    }

    void OpenUpgradeMenu()
    {
        if (upgradeMenu != null)
        {
            upgradeMenu.SetActive(true);
        }
    }
}