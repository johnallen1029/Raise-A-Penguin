

using System.Collections;
using Unity.AppUI.UI;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem; // Import the Input System namespace


public class PlayerMovement : MonoBehaviour
{
    Vector3 veloctiy;
    float gravity = -9.01f;
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

    // Camera-relative movement
    Transform cameraTransform = Camera.main.transform;
    Vector3 cameraForward = cameraTransform.forward;
    Vector3 cameraRight = cameraTransform.right;
    cameraForward.y = 0f;
    cameraRight.y = 0f;
    cameraForward.Normalize();
    cameraRight.Normalize();

    Vector3 relativeMovement = (cameraForward * movement.z + cameraRight * movement.x).normalized;
        transform.Translate(relativeMovement * currentSpeed * Time.deltaTime, Space.World);
        bool isWalking = movement.magnitude > 0;
        animator.SetBool("isWalking", isWalking); 
    // Always rotate penguin (not only while moving) so flips show in place
    // Always rotate penguin
Transform penguinTransform = transform.Find("penguingo");

if (penguinTransform != null)
{
    // Calculate yaw facing (based on movement if moving, otherwise keep last facing)
    float targetAngle = (relativeMovement.magnitude >= 0.1f)
        ? Mathf.Atan2(relativeMovement.x, relativeMovement.z) * Mathf.Rad2Deg
        : penguinTransform.localEulerAngles.y - 90f; // preserve last yaw

    float modelYawOffset = 90f;
    float yaw = targetAngle + modelYawOffset;

    // Build facing rotation (just yaw)
    Quaternion facingRot = Quaternion.Euler(0f, yaw, 0f);

    if (isFlipping)
        {
            // Apply full flip directly
            Quaternion flipRot = Quaternion.Euler(flipAngle, 0, 0);
            penguinTransform.localRotation = facingRot * flipRot;
        }
        else
        {
            // Smooth turn to face direction when not flipping
            penguinTransform.localRotation = Quaternion.Slerp(
                penguinTransform.localRotation,
                facingRot,
                Time.deltaTime * 10f
            );
        }
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
    isFlipping = true; 
    float flipDuration = 0.5f; // Duration of the flip
    float elapsedTime = 0f;

    // Store the initial rotation
    Transform penguinTransform = transform.Find("penguingo");
    if (penguinTransform == null)
    {
        Debug.LogError("Penguin transform not found!");
        yield break;
    }

    // Get the camera's forward direction
    Transform cameraTransform = Camera.main.transform;
    Vector3 cameraForward = cameraTransform.forward;
    cameraForward.y = 0f; // Ignore vertical component
    cameraForward.Normalize();

    // Calculate the forward-facing rotation relative to the camera
    Quaternion facingRot = Quaternion.LookRotation(cameraForward, Vector3.up);

    // Adjust the facing rotation to align the penguin forward
    Quaternion modelOffset = Quaternion.Euler(0f, 90f, 0f); // Adjust this value if needed
    facingRot = facingRot * modelOffset;

    // Add a delay before starting the flip
    yield return new WaitForSeconds(0.1f); // Adjust this value for more air time

    while (elapsedTime < flipDuration)
    {
        // Dynamically calculate the flip angle
        flipAngle = Mathf.Lerp(0f, 360f, elapsedTime / flipDuration);

        // Apply the flip around the Z-axis (forward axis relative to the camera)
        Quaternion flipRot = Quaternion.AngleAxis(flipAngle, Vector3.forward);
        penguinTransform.localRotation = facingRot * flipRot;

        elapsedTime += Time.deltaTime;
        yield return null;
    }

    // Reset rotation to original facing direction
    penguinTransform.localRotation = facingRot;

    isFlipping = false; 
    Debug.Log("Front flip completed.");
    animator.SetBool("isJumping", false);
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


}