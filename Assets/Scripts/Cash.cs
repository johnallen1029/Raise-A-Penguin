using UnityEngine;

public class Cash : MonoBehaviour
{
    public static int cashValue = 5;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.AddMoney(cashValue);
            Destroy(gameObject); 
        }
    }
}
