using UnityEngine;

public class PenguinHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    PenguinHunger penguinHunger;

    void Start()
    {
        currentHealth = maxHealth;
        penguinHunger = GetComponent<PenguinHunger>(); 
    }

    public void TakeDamage(float damage)
    {
        penguinHunger.TakeDamage(damage); 

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Penguin has been defeated!");
        // Add logic for penguin defeat (e.g., game over)
    }
}