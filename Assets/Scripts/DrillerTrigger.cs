using UnityEngine;
using UnityEngine.InputSystem; // Import the New Input System namespace
using TMPro;

[RequireComponent(typeof(BoxCollider))]
public class DrillerTrigger : MonoBehaviour
{
    public GameObject player; // Reference to the player object
    public TextMeshProUGUI promptText; // Reference to the UI text for the prompt
    public string interactionKey = "E"; // Key to interact with the driller

    private DrillerInteraction drillerInteraction;
    private bool isPlayerNearby = false;

    private void Start()
    {
        drillerInteraction = GetComponent<DrillerInteraction>();

        // Ensure the BoxCollider is set as a trigger
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            boxCollider.isTrigger = true;
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
                    StartDrilling();
                }
            }
        }
    }

    void StartDrilling()
    {
        if (drillerInteraction != null)
        {
            drillerInteraction.StartDrilling();
        }
    }

    void ShowPrompt(bool show)
    {
        if (promptText != null)
        {
            promptText.gameObject.SetActive(show);
            if (show)
            {
                promptText.text = $"Press {interactionKey} to start drilling";
            }
        }
    }
}