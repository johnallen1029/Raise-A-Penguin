using UnityEngine;

public class SealMovement : MonoBehaviour
{
    public Transform target;
    public float speed = 2f;

    private Rigidbody rb; 

    void Start()
    {
        rb = GetComponent<Rigidbody>(); 
    }
    void Update()
    {
        if (target != null && gameObject.tag != "MasterSeal")
        {
            Vector3 direction = (target.position - transform.position).normalized;


            rb.MovePosition(transform.position + direction * speed * Time.fixedDeltaTime); 
        }
    }
}
