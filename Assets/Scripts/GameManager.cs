using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int PlayerMoney { get; set; } = 0;
    public int PlayerFish { get; set; } = 0;

    public int PlayerPebble { get; set; } = 0; 
    public event Action OnMoneyChanged;
    public event Action OnFishChanged;

    public event Action OnPebbleChanged;

    public event Action acquiredPebble;
    public bool acquired = false; 

    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); //Persist 
    }

    public void AddMoney(int amount)
    {
        PlayerMoney += amount;
        Debug.Log($"Money added: {amount}. Total money: {PlayerMoney}");
        OnMoneyChanged?.Invoke();
    }
    public void ResetStats()
    {
        PlayerFish = 0;
        PlayerMoney = 0;
        PlayerPebble = 0; 
        Cash.cashValue = 5; 

    }

    public void AddFish(int amount)
    {
        PlayerFish += amount;
        OnFishChanged?.Invoke();
    }

    public bool SpendMoney(int amount)
    {
        if (PlayerMoney >= amount)
        {
            PlayerMoney -= amount;
            OnMoneyChanged?.Invoke();
            return true;

        }
        else
        {
            return false;
        }
    }

    public bool SpendFish(int amount)
    {
        if (PlayerFish >= amount)
        {
            PlayerFish -= amount;
            OnFishChanged?.Invoke();
            return true;
        }
        else
        {
            return false;
        }
    }
    public void AddPebble(int amount)
    {
        PlayerPebble += amount;
        OnPebbleChanged?.Invoke();

        if (PlayerPebble > 0 && !acquired)
        {
            acquiredPebble?.Invoke();
            acquired = true; 
        }
    }
    
    public bool SpendPebble(int amount)
    {
        if (PlayerPebble >= amount)
        {
            PlayerPebble -= amount;
            OnPebbleChanged?.Invoke();
            return true;
        }
        else
        {
            return false; 
        }
    }
    
}
