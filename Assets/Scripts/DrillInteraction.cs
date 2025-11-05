using UnityEngine;

public class DrillerInteraction : MonoBehaviour
{
    public Transform cylinder; // Assign the Cylinder object here

    public float targetYPosition = 97f;     // Where the drill stops (down)
    public float returnYPosition = 104.32f; // Starting height (up)
    public float drillDuration = 3f;        // Time to move each way
    public float spinSpeed = 720f;          // Rotation speed while drilling

    private bool isDrillingDown = false;
    private bool isReturning = false;
    private float elapsedTime = 0f;
    private Vector3 initialPosition;

    private void Start()
    {
        if (cylinder != null)
            initialPosition = cylinder.position;
    }

    private void Update()
    {
        if (isDrillingDown && cylinder != null)
        {
            // Spin while drilling down
            cylinder.Rotate(Vector3.up, spinSpeed * Time.deltaTime);

            // Move downward
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / drillDuration);
            float newY = Mathf.Lerp(initialPosition.y, targetYPosition, progress);
            cylinder.position = new Vector3(cylinder.position.x, newY, cylinder.position.z);

            // Finished drilling down
            if (progress >= 1f)
            {
                isDrillingDown = false;
                isReturning = true;
                elapsedTime = 0f; // Reset timer for return motion
            }
        }
        else if (isReturning && cylinder != null)
        {
            // Move upward — no spin this time
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / drillDuration);
            float newY = Mathf.Lerp(targetYPosition, returnYPosition, progress);
            cylinder.position = new Vector3(cylinder.position.x, newY, cylinder.position.z);

            // Finished returning
            if (progress >= 1f)
            {
                isReturning = false;
                elapsedTime = 0f;
                GameManager.Instance.AddPebble(1); 
            }
        }
    }

    public void StartDrilling()
    {
        if (!isDrillingDown && !isReturning)
        {
            isDrillingDown = true;
            elapsedTime = 0f;
            initialPosition = cylinder.position;
        }
    }
}
