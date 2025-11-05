using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro; // Import the New Input System namespace

public class ComputerInteraction : MonoBehaviour
{
    public GameObject player; // Reference to the player object
    public UpgradeMenu upgradeMenu; // Reference to the upgrade menu UI
    public TextMeshProUGUI promptText; // Reference to the UI text for the prompt
    public string interactionKey = "E"; // Key to interact with the computer
    public float interactionDistance = 3.0f; // Distance within which the player can interact

    private bool isPlayerNearby = false;

    void Update()
    {
        // Check the distance between the player and the computer
        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance <= interactionDistance)
        {
            if (!isPlayerNearby)
            {
                isPlayerNearby = true;
                ShowPrompt(true);
            }

            // Check if the interaction key is pressed using the New Input System
            Key keyEnum;
            if (System.Enum.TryParse<Key>(interactionKey.ToUpper(), out keyEnum))
            {
                var keyControl = Keyboard.current[keyEnum];
                if (keyControl != null && keyControl.wasPressedThisFrame)
                {
                    ShowPrompt(false);
                    OpenUpgradeMenu();
                }
            }
        }
        else
        {
            if (isPlayerNearby)
            {
                isPlayerNearby = false;
                ShowPrompt(false);
            }
        }
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
            upgradeMenu.OpenMenu();
        }
    }
}