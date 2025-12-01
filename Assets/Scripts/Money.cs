using UnityEngine;

public class Money : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.AddMoney(Cash.cashValue);
            SoundEffectManager.Instance.PlayMoneySound(); 
            
            Destroy(gameObject); 
        }
    }
}
