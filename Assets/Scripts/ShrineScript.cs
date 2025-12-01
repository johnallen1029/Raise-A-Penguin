// 12/1/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

// 11/30/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class ShrineInteractionWithMinigame : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public float interactionDistance = 5f; // Distance within which the player can interact
    public Slider timerSlider; // Reference to the slider UI element
    public TextMeshPro multiplierText; // Reference to the multiplier text UI element
    public TextMeshProUGUI promptText; // Reference to the interaction prompt text
    public GameObject squarePrefab; // Prefab for the squares
    public Canvas canvas; // Reference to the canvas for the minigame
    public float timerDuration = 10f; // Duration of the timer
    public float moneyMultiplier = 2f; // Multiplier for money
    public string interactionKey = "E"; // Key to interact with the shrine
    public Vector2 buttonSize = new Vector2(100, 100); // Size of the buttons
    public CameraFollow cameraFollow; // Reference to the CameraFollow script
    public Vector2 margin = new Vector2(100, 100); // Margins for button placement

    private bool isTimerActive = false;
    private bool isMinigameActive = false; // Flag to track if the minigame is active
    private float timer;
    private float minigameTimer = 0f; // Timer for the minigame duration
    private float minigameDuration = 3f; // Duration for the minigame to be active
    private int currentSequence = 1; // Current sequence number to click
    private GameObject[] squares; // Array to hold the spawned squares
    private bool isPlayerNearby = false;

    void Update()
    {
        // Billboard the text to always face the camera
        if (multiplierText != null)
        {
            multiplierText.transform.LookAt(Camera.main.transform);
            multiplierText.transform.Rotate(0, 180, 0); // Adjust rotation to face the camera
        }

        if (timerSlider != null)
        {
            timerSlider.transform.LookAt(Camera.main.transform);
            timerSlider.transform.Rotate(0, 180, 0);
        }

        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance <= interactionDistance && !isMinigameActive)
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
                    StartMinigame();
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

        // Update the timer and slider
        if (isTimerActive)
        {
            timer -= Time.deltaTime;
            timerSlider.value = Mathf.Clamp01(timer / timerDuration);

            if (timer <= 0)
            {
                EndTimer();
            }
        }

        // Update the minigame timer
        if (isMinigameActive)
        {
            minigameTimer += Time.deltaTime;

            if (minigameTimer >= minigameDuration)
            {
                ResetMinigame(); // Minigame failed due to timeout
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

    void StartMinigame()
    {
        if (!isTimerActive && !isMinigameActive)
        {
            isMinigameActive = true; // Set the flag to block further activations
            minigameTimer = 0f; // Reset the minigame timer

            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.canMove = false;
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (cameraFollow != null)
            {
                cameraFollow.PauseCamera();
            }

            promptText.text = "Click the squares in order!";
            SpawnSquares();
        }
    }

    void SpawnSquares()
    {
        squares = new GameObject[5];
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        // Calculate the boundaries for button placement considering margins
        float minX = -canvasRect.rect.width / 2 + margin.x;
        float maxX = canvasRect.rect.width / 2 - margin.x;
        float minY = -canvasRect.rect.height / 2 + margin.y;
        float maxY = canvasRect.rect.height / 2 - margin.y;

        for (int i = 0; i < 5; i++)
        {
            GameObject square = Instantiate(squarePrefab, canvas.transform);
            TextMeshProUGUI textComponent = square.GetComponentInChildren<TextMeshProUGUI>();
            textComponent.text = (i + 1).ToString();
            textComponent.fontSize = 36; // Set the font size to make the text bigger
            square.GetComponent<Button>().onClick.AddListener(() => OnSquareClicked(int.Parse(textComponent.text)));
            squares[i] = square;

            // Randomize position within the canvas with margins and ensure no overlap
            RectTransform rectTransform = square.GetComponent<RectTransform>();
            rectTransform.sizeDelta = buttonSize; // Set button size

            Vector2 position;
            bool isOverlapping;
            do
            {
                isOverlapping = false;
                position = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));

                foreach (GameObject otherSquare in squares)
                {
                    if (otherSquare != null)
                    {
                        RectTransform otherRect = otherSquare.GetComponent<RectTransform>();
                        if (Vector2.Distance(position, otherRect.anchoredPosition) < buttonSize.x)
                        {
                            isOverlapping = true;
                            break;
                        }
                    }
                }
            } while (isOverlapping);

            rectTransform.anchoredPosition = position;
        }
    }

    void OnSquareClicked(int number)
    {
        if (number == currentSequence)
        {
            // Change color of the clicked square
            squares[number - 1].GetComponent<Image>().color = Color.green;

            currentSequence++;
            if (currentSequence > 5)
            {
                ActivateMultiplier();
            }
        }
        else
        {
            ResetMinigame();
        }
    }

    void ActivateMultiplier()
    {
        isTimerActive = true;
        timer = timerDuration;
        multiplierText.text = "Mult: 2x"; // Update multiplier text
        GameManager.Instance.MoneyMult = 2;
        promptText.text = "Mult: 1x"; // Clear prompt text
        foreach (GameObject square in squares)
        {
            Destroy(square); // Remove squares
        }

        ReturnControlToPlayer();
    }

    void ResetMinigame()
    {
        currentSequence = 1;
        foreach (GameObject square in squares)
        {
            Destroy(square); // Remove squares
        }
        promptText.text = "Minigame failed! Try again.";

        ReturnControlToPlayer();
    }

    void EndTimer()
    {
        isTimerActive = false;
        timer = 0;
        timerSlider.value = 0;
        multiplierText.text = ""; // Reset multiplier text
        GameManager.Instance.MoneyMult = 1; // Reset money multiplier
    }

    void ReturnControlToPlayer()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraFollow != null)
        {
            cameraFollow.ResumeCamera();
        }

        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.canMove = true;

            playerMovement.ResetMovement(); 
        }

        isMinigameActive = false; // Reset the minigame active flag
    }
}