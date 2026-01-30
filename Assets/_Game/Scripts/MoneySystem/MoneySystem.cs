using System;
using UnityEngine;

public class MoneySystem : MonoBehaviour
{
    public static MoneySystem Instance;
    public event Action OnMoneyChanged;

    private int _wallet;
    private int _totalEarned;
    public int Wallet => _wallet;
    public int TotalEarned => _totalEarned;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        _wallet = 0;
        _totalEarned = 0;
    }

    public void Add(int amount)
    {
        _wallet += amount;
        _totalEarned += amount;
        OnMoneyChanged?.Invoke();
    }

    private void Minus(int amount)
    {
        _wallet -= amount;
        OnMoneyChanged?.Invoke();
    }

    public void Buy(int price)
    {
        if(_wallet >= price) Minus(price);
    }

    public void ResetRun()
    {
        _wallet = 0;
        _totalEarned = 0;
        OnMoneyChanged?.Invoke();
    }
    
}
