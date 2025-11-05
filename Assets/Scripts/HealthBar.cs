using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider; // Reference to the Slider UI
    public Transform target; // The NPC to follow
    public Vector3 offset = new Vector3(0, 2, 0); // Offset above the NPC

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Make the health bar face the camera
        transform.LookAt(transform.position + mainCamera.transform.forward);

        // Position the health bar above the NPC
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }

    public void SetHealth(float currentHealth, float maxHealth)
    {
        healthSlider.value = currentHealth / maxHealth;
    }
}