using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    public Vector3 pointA = new Vector3(-22.40f, -0.08f, 9f); // Starting point
    public Vector3 pointB = new Vector3(-22.40f, -0.08f, -22f); // End point
    public float speed = 4f; // Speed of movement

    private bool movingToB = true;
    

    void Update()
    {
        // Move the platform between pointA and pointB
        if (movingToB)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, pointB, speed * Time.deltaTime);
            if (Vector3.Distance(transform.localPosition, pointB) < 0.1f)
            {
                movingToB = false; // Switch direction
            }
        }
        else
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, pointA, speed * Time.deltaTime);
            if (Vector3.Distance(transform.localPosition, pointA) < 0.1f)
            {
                movingToB = true; // Switch direction
            }
        }
    }
}