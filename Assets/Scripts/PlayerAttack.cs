using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public float punchDamage = 25f;
    public float punchRange = 4f;
    public LayerMask enemyLayer;

    public string punchKey = "F";

    public Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>(); 
    }
    void Update()
    {
        // Check if the punch key is pressed using the New Input System
        Key keyEnum;
        if (System.Enum.TryParse<Key>(punchKey.ToUpper(), out keyEnum))
        {
            var keyControl = Keyboard.current[keyEnum];
            if (keyControl != null && keyControl.wasPressedThisFrame)
            {
                animator.SetTrigger("Punching");
                Punch();
            }
        }
    }

    void Punch()
    {
        // Check for enemies in range
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, punchRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(punchDamage);
                Debug.Log("PUNCHED");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw the punch range in the Scene view
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, punchRange);
    }
}