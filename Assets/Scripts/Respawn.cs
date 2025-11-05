using UnityEngine;

public class RespawnZone : MonoBehaviour
{
    public Transform respawnPoint; // Assign a respawn point in the Inspector

    private int currentFish;
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the player
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player fell off the platform! Respawning...");
            RespawnPlayer(other.gameObject);
        }
    }

    private void RespawnPlayer(GameObject player)
    {
        // Move the player to the respawn point
        player.transform.position = respawnPoint.position;

        currentFish = GameManager.Instance.PlayerFish;

        GameManager.Instance.AddFish(-currentFish);    

        // Optional: Reset player's velocity if they have a Rigidbody
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
        }
    }
}