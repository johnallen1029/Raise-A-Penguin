using System.Collections;
//using Unity.AppUI.UI;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem; // Import the Input System namespace


public class PlayerMovement : MonoBehaviour
{
    Vector3 veloctiy;
    private Animator animator; 
    public float sprintMult = 2f;
    public float speed = 5f;
    public float jumpForce = 10f;
    private Vector2 movementInput;
    private Rigidbody rb;
    private bool isGrounded = true;
    private bool isSprinting = false;

    private float flipAngle = 0f;

    private bool isFlipping = false;

    [SerializeField] private float modelFacingOffset = 90f; 
    public bool canMove = true; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>(); 
        if (rb == null)
        {
            Debug.LogError("Rigidbody componenet is missing on the player!");
        }
    }

 void Update()
{
    if (!canMove) return; 

    float currentSpeed = isSprinting ? speed * sprintMult : speed;
    Vector3 movement = new Vector3(movementInput.x, 0f, movementInput.y);

    // --- Camera-relative movement ---
    Transform cameraTransform = Camera.main.transform;
    Vector3 cameraForward = cameraTransform.forward;
    Vector3 cameraRight = cameraTransform.right;

    // Ignore camera tilt
    cameraForward.y = 0f;
    cameraRight.y = 0f;
    cameraForward.Normalize();
    cameraRight.Normalize();

    // Convert input to world-space relative to camera
    Vector3 relativeMovement = (cameraForward * movement.z + cameraRight * movement.x).normalized;

    // --- Move player GameObject ---
    transform.Translate(relativeMovement * currentSpeed * Time.deltaTime, Space.World);

    // --- Animate walk ---
    bool isWalking = movement.magnitude > 0;
    animator.SetBool("isWalking", isWalking);

    // --- Face the direction of movement (using GameObject, not model) ---
    if (relativeMovement.magnitude >= 0.1f && !isFlipping)
    {
            // Smoothly rotate the entire player GameObject toward movement direction
            Quaternion targetRotation = Quaternion.LookRotation(relativeMovement, Vector3.up);

            Quaternion correctedRotation = targetRotation * Quaternion.Euler(0f, modelFacingOffset, 0f); 

        transform.rotation = Quaternion.Slerp(transform.rotation, correctedRotation, Time.deltaTime * 10f);
    }
}

    // This method is called by the Input System when movement input is detected
    public void OnMove(InputValue value)
    {
        if (!canMove) return; 
        movementInput = value.Get<Vector2>();
        
    }
    public void ResetJumping()
    {
        if (animator != null)
        {
            animator.SetBool("isJumping", false); 
        }
    }

    public void OnJump(InputValue value)
    {
        if (isGrounded && canMove)
        {
            Vector3 v = rb.linearVelocity;
            v.y = 0;
            rb.linearVelocity = v; 

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetBool("isJumping", true);
            isGrounded = false;

            StartCoroutine(PerformFrontFlip()); 
        }

        if (!isSprinting)
        {
            Debug.Log("NOT SPRINTING");
        }
    }

private IEnumerator PerformFrontFlip()
{
    Debug.Log("Starting front flip...");
    SoundEffectManager.Instance.PlayJumpSound();  
    isFlipping = true; 
    float flipDuration = 0.75f; // Slow it down
    float elapsedTime = 0f;

    // Get the camera's forward direction (ignore vertical tilt)
    Transform cameraTransform = Camera.main.transform;
    Vector3 cameraForward = cameraTransform.forward;
    cameraForward.y = 0f;
    cameraForward.Normalize();

    // The facing direction for the player
    Quaternion facingRot = Quaternion.LookRotation(cameraForward, Vector3.up) * Quaternion.Euler(0f, modelFacingOffset, 0f);

    // Store the starting rotation
    Quaternion startRot = transform.rotation;

    while (elapsedTime < flipDuration)
    {
        float t = elapsedTime / flipDuration;
        // Calculate rotation progress (0° → 360°)
        float flipAngle = Mathf.Lerp(0f, 360f, t);

        // Rotate around the Z-axis (forward flip)
        // Note: Z-axis here means *local forward axis* of the player
        Quaternion flipRot = Quaternion.AngleAxis(flipAngle, Vector3.forward);

        // Combine facing and flip rotation
        transform.rotation = facingRot * flipRot;

        elapsedTime += Time.deltaTime;
        yield return null;
    }

    // Reset rotation to facing direction after the flip
    transform.rotation = facingRot;

    isFlipping = false;
    animator.SetBool("isJumping", false);
    Debug.Log("Front flip (Z-axis) completed.");
}


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("isJumping", false);
        }
    }
    public void OnSprint(InputValue value)
    {
        if (!canMove) return; 
        isSprinting = value.isPressed;
        Debug.Log("Sprint: " + (isSprinting ? "ON" : "OFF"));
    }

    public void ResetMovement()
    {
        movementInput = Vector2.zero; 
    }


}