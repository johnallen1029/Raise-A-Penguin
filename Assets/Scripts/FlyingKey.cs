using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class FlyingKey : MonoBehaviour
{
    private Vector3 targetPosition;
    private float speed;
    private Key keyType;
    private RectTransform rectTransform;
    private bool wasHit = false;
    private TextMeshProUGUI keyText;
    private Image keyBackground;
    private Vector3 startPosition;
    private float journeyLength;
    private float startTime;

    // Hit window settings
    private bool isInHitWindow = false;
    private float hitWindowStartDistance = 50f; // Start hit window when this close to target
    private float hitWindowEndDistance = 50f;   // End hit window when this far PAST target
    private bool hasPassedTarget = false;
    private Vector3 direction; // Direction of movement

    public Key KeyType => keyType;
    public bool IsInHitWindow => isInHitWindow && !wasHit;

    public void Initialize(Vector3 targetPos, float moveSpeed, Key key)
    {
        // Get the RectTransform
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("FlyingKey: No RectTransform found!");
            return;
        }

        targetPosition = targetPos;
        speed = moveSpeed;
        keyType = key;
        
        // Store start position and calculate journey
        startPosition = rectTransform.position;
        direction = (targetPosition - startPosition).normalized;
        journeyLength = Vector3.Distance(startPosition, targetPosition);
        startTime = Time.time;
        
        // Get references to components
        keyText = GetComponentInChildren<TextMeshProUGUI>();
        keyBackground = GetComponent<Image>();
        
        // Set the key text
        if (keyText != null)
        {
            keyText.text = GetKeyDisplayText(key);
        }
        else
        {
            Debug.LogError("FlyingKey: No TextMeshProUGUI found in children!");
        }

        // Set color based on key type
        if (keyBackground != null)
        {
            keyBackground.color = GetKeyColor(key);
        }

        Debug.Log($"FlyingKey initialized: {key} from {startPosition} to {targetPosition}, distance: {journeyLength}");
    }

    void Update()
    {
        if (wasHit || rectTransform == null) return;

        // Calculate distance traveled
        float distanceTraveled = (Time.time - startTime) * speed;
        
        // Move continuously in the direction of the target (will overshoot)
        rectTransform.position = startPosition + direction * distanceTraveled;

        // Calculate current distance to target
        float currentDistance = Vector3.Distance(rectTransform.position, targetPosition);
        
        // Check if we've passed the target
        float dotProduct = Vector3.Dot(direction, (rectTransform.position - targetPosition).normalized);
        if (!hasPassedTarget && dotProduct > 0f) // If moving away from target
        {
            hasPassedTarget = true;
            Debug.Log($"Key {keyType} passed target position! Current distance: {currentDistance}");
        }
        
        // Update hit window status
        UpdateHitWindow(currentDistance);

        // Destroy if well past target (missed by player)
        if (hasPassedTarget && currentDistance > hitWindowEndDistance)
        {
            Debug.Log($"Key {keyType} missed - {currentDistance}px past target (limit: {hitWindowEndDistance})");
            Destroy(gameObject);
        }
    }

    private void UpdateHitWindow(float currentDistance)
    {
        bool wasInHitWindow = isInHitWindow;
        
        // Hit window logic:
        // - Before reaching target: when within hitWindowStartDistance
        // - After passing target: when within hitWindowEndDistance past target
        if (!hasPassedTarget)
        {
            // Before passing target - use distance to target
            isInHitWindow = currentDistance <= hitWindowStartDistance;
        }
        else
        {
            // After passing target - use distance past target
            isInHitWindow = currentDistance <= hitWindowEndDistance;
        }

        // Visual feedback when entering/exiting hit window
        if (isInHitWindow && !wasInHitWindow)
        {
            // Just entered hit window - change color to yellow
            if (keyBackground != null)
                keyBackground.color = Color.yellow;
                
            Debug.Log($"Key {keyType} entered hit window! (Distance: {currentDistance}, PassedTarget: {hasPassedTarget})");
        }
        else if (!isInHitWindow && wasInHitWindow)
        {
            // Just exited hit window - change back to original color
            if (keyBackground != null)
                keyBackground.color = GetKeyColor(keyType);
                
            Debug.Log($"Key {keyType} exited hit window! (Distance: {currentDistance}, PassedTarget: {hasPassedTarget})");
        }
    }

    public void Hit()
    {
        if (wasHit) return;
        wasHit = true;
        
        Debug.Log($"Key {keyType} hit successfully!");
        
        // Visual feedback for successful hit
        if (keyBackground != null)
            keyBackground.color = Color.green;
        
        Destroy(gameObject, 0.1f);
    }

    private string GetKeyDisplayText(Key key)
    {
        switch (key)
        {
            case Key.W: return "W";
            case Key.A: return "A";
            case Key.S: return "S";
            case Key.D: return "D";
            default: return "?";
        }
    }

    private Color GetKeyColor(Key key)
    {
        switch (key)
        {
            case Key.W: return new Color(0.7f, 0.8f, 1f);   // Light Blue
            case Key.A: return new Color(1f, 0.8f, 0.7f);   // Light Orange  
            case Key.S: return new Color(0.8f, 1f, 0.7f);   // Light Green
            case Key.D: return new Color(1f, 0.7f, 0.9f);   // Light Pink
            default: return Color.white;
        }
    }
}