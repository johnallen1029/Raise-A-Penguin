using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public float damageToPenguin = 20f;

    private bool hasDealtDamage = false;
    public Transform penguin;

    void Start()
    {
        currentHealth = maxHealth;
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        gameObject.SetActive(false);
        ResetState();
    }
    void OnTriggerEnter(Collider other)
    {
        if (hasDealtDamage) return;
        if (other.CompareTag("Penguin"))
        {
            PenguinHealth penguinHealth = other.GetComponent<PenguinHealth>();
            if (penguinHealth != null)
            {
                penguinHealth.TakeDamage(damageToPenguin);
            }
            Debug.Log("DAMGE DONE");
            hasDealtDamage = true;
            Die();
        }
    }
    void ResetState()
    {
        currentHealth = maxHealth;
        hasDealtDamage = false; 
    }
}
