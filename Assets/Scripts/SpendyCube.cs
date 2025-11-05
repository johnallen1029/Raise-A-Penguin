using UnityEngine;

public class SpendyCube : MonoBehaviour
{
    public int spendValue = 5;

    void OnCollisionEnter(Collision collision)
    {
        GameManager.Instance.SpendMoney(spendValue); 
    }

}
