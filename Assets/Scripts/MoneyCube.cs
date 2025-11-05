using UnityEngine;

public class MoneyCube : MonoBehaviour
{
    public int moneyValue = 5;
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.AddMoney(moneyValue);
        }
    }

}
