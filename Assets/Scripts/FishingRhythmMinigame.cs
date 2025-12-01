using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class FishingRhythmMinigame : MonoBehaviour
{
    public GameObject player;
    public GameObject fishingTarget;
    public GameObject keyPrefab;
    public Transform[] keySpawnPoints; // Starting positions for each key (WASD)
    public Transform[] keyTargets; // End positions for each key (WASD)
    public float keySpeed = 5f;
    public float sequenceTime = 3f;
    public int fishAdd = 1;

    [SerializeField] public TextMeshProUGUI sequenceText;

    private bool isFishing = false;
    private bool sequenceActive = false;
    private List<Key> keySequence = new List<Key>();
    private int currentKeyIndex = 0;
    private List<FlyingKey> activeFlyingKeys = new List<FlyingKey>();

    private Dictionary<Key, int> keyToIndex = new Dictionary<Key, int>
    {
        { Key.W, 0 },
        { Key.A, 1 },
        { Key.S, 2 },
        { Key.D, 3 }
    };

    private Dictionary<Key, string> keyToDirection = new Dictionary<Key, string>
    {
        { Key.W, "W" },
        { Key.A, "A" },
        { Key.S, "S" },
        { Key.D, "D" }
    };

    private Dictionary<Key, float> keyToPitch = new Dictionary<Key, float>
    {
        { Key.W, 1.0f },    // C (root)
        { Key.A, 1.122f },  // D 
        { Key.S, 1.335f },  // G
        { Key.D, 1.414f }   // A
    };

    void Update()
    {
        if (Keyboard.current[Key.E].wasPressedThisFrame && !isFishing)
        {
            StartFishing();
        }

        if (sequenceActive)
        {
            HandleRhythmInput();
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
        StartCoroutine(StartRhythmSequence());
    }

    private IEnumerator StartRhythmSequence()
    {
        sequenceActive = true;
        currentKeyIndex = 0;
        int sequenceLength = 3; 
        int fishreward = 1; 
        activeFlyingKeys.Clear();

        // Generate random sequence of directional keys
        keySequence.Clear();
        if (fishingTarget != null && fishingTarget.CompareTag("FisherBig"))
        {
            sequenceLength = 6; 
        }
        for (int i = 0; i < sequenceLength; i++)
        {
            Key randomKey = GetRandomDirectionalKey();
            keySequence.Add(randomKey);
        }

        sequenceText.text = "Get ready!";
        Debug.Log($"Rhythm sequence started! Sequence: {GetSequenceString()}");

        yield return new WaitForSeconds(1f);

        sequenceText.text = " "; 
        //UpdateSequenceDisplay();

        // Spawn all keys with delays between them
        for (int i = 0; i < keySequence.Count; i++)
        {
            SpawnFlyingKey(keySequence[i]);
            yield return new WaitForSeconds(sequenceTime / keySequence.Count);
        }

        // Wait for the sequence to complete or timeout
        yield return new WaitForSeconds(sequenceTime);

        // If the sequence is not completed in time, fail the minigame
        if (currentKeyIndex < keySequence.Count)
        {
            Debug.Log("Failed to catch the fish - Time's up!");
            sequenceText.text = "Too slow! Failed!";
            EndMinigame(false);
        }
    }

    private void SpawnFlyingKey(Key key)
    {
        int keyIndex = keyToIndex[key];
        
        // Make sure we have a key prefab
        if (keyPrefab == null)
        {
            Debug.LogError("Key prefab is not assigned!");
            return;
        }

        // Use the spawn point for this specific key
        Vector3 spawnPosition = keySpawnPoints[keyIndex].position;
        
        Debug.Log($"Spawning {key} at {spawnPosition}, targeting {keyTargets[keyIndex].position}");
        
        GameObject flyingKeyObj = Instantiate(keyPrefab, spawnPosition, Quaternion.identity, transform);
        
        FlyingKey flyingKey = flyingKeyObj.GetComponent<FlyingKey>();
        
        if (flyingKey != null)
        {
            // Pass the target key's position and type
            flyingKey.Initialize(keyTargets[keyIndex].position, keySpeed, key);
            activeFlyingKeys.Add(flyingKey);
        }
        else
        {
            Debug.LogError("FlyingKey component not found on instantiated prefab!");
            Destroy(flyingKeyObj);
        }
    }

    private void HandleRhythmInput()
{
    if (currentKeyIndex >= keySequence.Count) return;

    // Check all directional keys
    foreach (Key key in keyToIndex.Keys)
    {
        if (Keyboard.current[key].wasPressedThisFrame)
        {
            // Check if this is the correct key for the sequence
            if (key == keySequence[currentKeyIndex])
            {
                // Check if there's an active flying key of this type IN THE HIT WINDOW
                FlyingKey flyingKey = GetActiveFlyingKeyInHitWindow(key);
                if (flyingKey != null)
                {
                    
                    flyingKey.Hit(); // Mark key as hit
                    SoundEffectManager.Instance.SetSFXVolume(0.5f);
                    SoundEffectManager.Instance.PlayKeyHit(keyToPitch[key]); 
                    SoundEffectManager.Instance.SetSFXVolume(0.04f); 
                    activeFlyingKeys.Remove(flyingKey);
                    currentKeyIndex++;
                    //UpdateSequenceDisplay();

                    // Check if sequence is complete
                    if (currentKeyIndex >= keySequence.Count)
                    {
                        Debug.Log("Success! Fish caught!");
                        sequenceText.text = "Success! Fish caught!";
                        EndMinigame(true);
                    }
                }
                else
                {
                    // Key was pressed but no active flying key of this type is in hit window
                    Debug.Log($"Wrong timing for key: {key} - pressed but not in hit window");
                    sequenceText.text = "Bad timing! Failed!";
                    EndMinigame(false);
                }
            }
            else
            {
                // Wrong key pressed
                Debug.Log($"Wrong key pressed: {key}");
                sequenceText.text = "Wrong key! Failed!";
                EndMinigame(false);
            }
            break; // Only process one key press per frame
        }
    }
}

private FlyingKey GetActiveFlyingKeyInHitWindow(Key key)
{
    foreach (FlyingKey flyingKey in activeFlyingKeys)
    {
        if (flyingKey != null && flyingKey.KeyType == key && flyingKey.IsInHitWindow)
        {
            return flyingKey;
        }
    }
    return null;
}

    private void UpdateSequenceDisplay()
    {
        string updatedSequence = "";

        for (int i = 0; i < keySequence.Count; i++)
        {
            if (i < currentKeyIndex)
            {
                // Completed keys are green with strikethrough
                updatedSequence += $"<color=green><s>{keyToDirection[keySequence[i]]}</s></color> ";
            }
            else
            {
                // Current and upcoming keys
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
            sequence += keyToDirection[key] + " ";
        }
        return sequence.Trim();
    }

    public void EndMinigame(bool success)
    {
        // Clean up all active flying keys
        foreach (FlyingKey key in activeFlyingKeys)
        {
            if (key != null)
                Destroy(key.gameObject);
        }
        activeFlyingKeys.Clear();

        isFishing = false;
        sequenceActive = false;
        currentKeyIndex = 0;
        keySequence.Clear();
        int fishReward = 1; 

        if (fishingTarget != null && fishingTarget.CompareTag("FisherBig"))
        {
            fishReward = 10; 
        }

        if (success)
        {
            // Add fish to inventory
            Debug.Log($"Added {fishAdd} fish to inventory!");
            GameManager.Instance.AddFish(fishReward); 
            // Add your fish collection logic here
        }

        sequenceText.text = "";
        gameObject.SetActive(false);
        
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.canMove = true;
        }
        Debug.Log("Minigame ended.");
    }

    
}