
using UnityEngine;

public class SentryBehavior : MonoBehaviour
{
    public GameObject snowballPrefab; // Assign the Snowball prefab in the Inspector
    public Transform snowballSpawnPoint; // Where the snowball will spawn
    public float detectionRange = 15f; // Range to detect enemies
    public float snowballSpeed = 500; // Speed of the snowball
    public float fireRate = 1f; // Time between snowball throws

    private float nextFireTime = 2f;

    private void OnTriggerStay(Collider other)
    {
       
        // Check if the object within the trigger is a leopardseal
        if (other.CompareTag("Enemy"))
        {
            if (Time.time >= nextFireTime)
            {
                ThrowSnowball(other.transform);
                nextFireTime = Time.time + fireRate;
            }
        }
    }


// 10/13/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

private void ThrowSnowball(Transform target)
{
    Debug.Log("throwing");
    GameObject snowball = Instantiate(snowballPrefab, snowballSpawnPoint.position, Quaternion.identity);

    // Calculate direction to the target
    Vector3 direction = (target.position - snowballSpawnPoint.position).normalized;

    // Get the Rigidbody of the snowball
    Rigidbody rb = snowball.GetComponent<Rigidbody>();

        // Disable gravity to ensure the snowball travels in a straight line
        rb.useGravity = false;
        rb.linearDamping = 0f; 

    // Set the velocity directly to ensure it hits the target
    rb.linearVelocity = direction * snowballSpeed;

    // Destroy the snowball after a few seconds
    Destroy(snowball, 3f);
}

    private void OnDrawGizmosSelected()
    {
        // Visualize the detection range in the Scene view
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}