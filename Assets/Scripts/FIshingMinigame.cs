// 10/27/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FishingMinigame : MonoBehaviour
{
    public Slider progressBar;

    public GameObject player;
    public GameObject fishingTarget;
    public RectTransform targetZone;
    public float moveSpeed = 2f;

    public int fishAdd = 1; 

    public string interactionKey = "E"; // Key to start fishing and set the hook
    [SerializeField] public TextMeshProUGUI sequenceText; // UI element to display the sequence
    public float biteTimeMin = 0.5f; // Minimum time for fish to bite
    public float biteTimeMax = 1.5f; // Maximum time for fish to bite
    public float sequenceTime = 3f; // Time to complete the sequence
    public float hookSetTime = 0.5f;

    private bool isFishing = false;
    private bool fishBit = false;
    private bool sequenceActive = false;
    private bool hookSetTimerActive = false;
    private List<Key> keySequence = new List<Key>();
    private int currentKeyIndex = 0;

    private Dictionary<Key, string> keyToDirection = new Dictionary<Key, string>
    {
        { Key.W, "Up" },
        { Key.A, "Left" },
        { Key.S, "Down" },
        { Key.D, "Right" }
    };


    void Update()
    {
        if (Keyboard.current[Key.E].wasPressedThisFrame && !isFishing)
        {
            StartFishing();
        }

        if (fishBit && !sequenceActive && hookSetTimerActive)
        {
            Key keyEnum;
            if (System.Enum.TryParse<Key>(interactionKey.ToUpper(), out keyEnum))
            {
                var keyControl = Keyboard.current[keyEnum];
                if (keyControl != null && keyControl.wasPressedThisFrame)
                {
                    hookSetTimerActive = false;
                    StartCoroutine(StartSequence());
                }
            }
        }

        if (sequenceActive)
        {
            HandleSequenceInput();
        }
    }

    public void StartFishing()
    {
        Debug.Log("Fishing started!");
        isFishing = true;
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.canMove = false;
        }
        StartCoroutine(FishBiteTimer());
    }



    private IEnumerator FishBiteTimer()
    {
        // Ensure the coroutine doesn't run if the sequence is active
        if (sequenceActive) yield break;

        float biteTime = Random.Range(biteTimeMin, biteTimeMax);
        sequenceText.text = "Waiting for fish to bite...";
        yield return new WaitForSeconds(biteTime);

        // Only set fishBit to true if the sequence is not active
        if (!sequenceActive)
        {
            fishBit = true;
            hookSetTimerActive = true;
            sequenceText.text = "Fish bit! Press E to set the hook!";
            StartCoroutine(HookSetTimer());
        }
    }

    private IEnumerator HookSetTimer()
    {
        float elapsedTime = 0f;

        while (elapsedTime < hookSetTime)
        {
            if (!hookSetTimerActive) yield break; // Stop timer if hook is set
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // If the timer runs out and the hook is not set, fail the minigame
        if (hookSetTimerActive)
        {
            Debug.Log("Failed to set the hook in time!");
            sequenceText.text = "Failed to set the hook!";
            EndMinigame();
        }
    }
    private string GetColoredSequenceString()
{
    string sequence = "";
    foreach (Key key in keySequence)
    {
        sequence += $"<color=white>{keyToDirection[key]}</color> ";
    }
    return sequence.Trim();
}

    private IEnumerator StartSequence()
{
    fishBit = false;
    sequenceActive = true;

    // Determine sequence length and time based on the tag of the fishingTarget
    int sequenceLength = 3; // Default sequence length
    float currentSequenceTime = sequenceTime; // Default sequence time

    if (fishingTarget.CompareTag("FisherBig"))
    {
        sequenceLength = 6; // Double the sequence length
            currentSequenceTime = sequenceTime * 2; // Double the sequence time
            fishAdd = 10; 
    }

    // Generate random sequence of directional keys
    keySequence.Clear();
    for (int i = 0; i < sequenceLength; i++)
    {
        Key randomKey = GetRandomDirectionalKey();
        keySequence.Add(randomKey);
    }

        // Display the sequence to the player
        sequenceText.text = GetColoredSequenceString(); 

    Debug.Log($"Sequence started! Enter the correct keys! Length: {sequenceLength}, Time: {currentSequenceTime}");
    float elapsedTime = 0f;

    while (elapsedTime < currentSequenceTime)
    {
        elapsedTime += Time.deltaTime;
        yield return null;

        // Check if the sequence is completed
        if (currentKeyIndex >= keySequence.Count)
        {
            sequenceText.text = "Success! You caught the fish!";
            GameManager.Instance.AddFish(fishAdd); // Add fish to inventory
            EndMinigame();
            yield break;
        }
    }

    // If time runs out, fail the minigame
    Debug.Log("Failed to catch the fish!");
    sequenceText.text = "Failed to catch the fish!";
    EndMinigame();
}

    private void HandleSequenceInput()
    {
        if (currentKeyIndex < keySequence.Count)
        {
            Key currentKey = keySequence[currentKeyIndex];
            foreach (Key key in keyToDirection.Keys) // Check all directional keys
            {
                var keyControl = Keyboard.current[key];
                if (keyControl != null && keyControl.wasPressedThisFrame)
                {
                    if (key == currentKey)
                    {
                        // Correct key pressed
                        Debug.Log($"Correct key pressed: {key}");
                        currentKeyIndex++;
                        UpdateSequenceDisplay(); // Update the sequence display
                    }
                    else
                    {
                        // Wrong key pressed, fail the minigame
                        Debug.Log($"Wrong key pressed: {key}. Expected: {currentKey}");
                        sequenceText.text = "Failed to catch the fish!";
                        EndMinigame();
                    }
                    break; // Exit the loop after detecting a key press
                }
            }
        }
    }

private void UpdateSequenceDisplay()
{
    string updatedSequence = "";

    for (int i = 0; i < keySequence.Count; i++)
    {
        if (i < currentKeyIndex)
        {
            // Correctly entered keys are green
            updatedSequence += $"<color=green>{keyToDirection[keySequence[i]]}</color> ";
        }
        else
        {
            // Remaining keys are white
            updatedSequence += $"<color=white>{keyToDirection[keySequence[i]]}</color> ";
        }
    }

    sequenceText.text = updatedSequence.Trim();
}
    private Key GetRandomDirectionalKey()
    {
        Key[] directionalKeys = { Key.W, Key.A, Key.S, Key.D };
        return directionalKeys[Random.Range(0, directionalKeys.Length)];
    }

    private string GetSequenceString()
    {
        string sequence = "";
        foreach (Key key in keySequence)
        {
            sequence += key.ToString() + " ";
        }
        return sequence.Trim();
    }

    public void EndMinigame()
    {
        isFishing = false;
        fishBit = false;
        sequenceActive = false;
        currentKeyIndex = 0;
        keySequence.Clear();
        sequenceText.text = ""; // Clear the sequence display
        gameObject.SetActive(false);
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>(); 
        if (playerMovement != null)
        {
            playerMovement.canMove = true; 
        }
        Debug.Log("Minigame ended.");
    }
}