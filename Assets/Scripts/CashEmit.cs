using UnityEngine;

public class CashEmit : MonoBehaviour
{
    public GameObject Money;
    public float emitInterval = 2f;
    public Transform emitPoint;

    private void Start()
    {
        StartEmitting();
    }

    public void UpdateEmitInterval(float newInterval)
    {
        emitInterval = newInterval;
        CancelInvoke(nameof(EmitCash)); // Cancel the current InvokeRepeating
        StartEmitting(); // Start a new InvokeRepeating with the updated interval
    }

    private void StartEmitting()
    {
        InvokeRepeating(nameof(EmitCash), 0f, emitInterval);
    }

    private void EmitCash()
    {
        if (Money != null)
        {
            Vector3 spawnPosition = emitPoint != null ? emitPoint.position : transform.position;
            Instantiate(Money, spawnPosition, Quaternion.identity);    
        }
    }
}