// 10/13/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using UnityEngine;

public class SnowballBehavior : MonoBehaviour
{
    public int damage = 100; // Damage dealt to the seal

    private void OnCollisionEnter(Collision collision)
    {
            Debug.Log($"Collided with: {collision.gameObject.name}, Tag: {collision.gameObject.tag}");
        Debug.Log("Enter collision");
        // Check if the object hit is tagged as "MasterSeal"
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("ISENEMY"); 
            // Get the EnemyHealth component on the seal
            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                // Apply damage to the seal
                enemyHealth.TakeDamage(damage);
                Debug.Log("did damage!");
            }

            // Destroy the snowball after it hits
            Destroy(gameObject);
        }
    }
}