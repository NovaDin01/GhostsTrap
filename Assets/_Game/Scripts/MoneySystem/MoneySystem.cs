using System;
using UnityEngine;

public class MoneySystem : MonoBehaviour
{
    public static MoneySystem Instance;
    private int _wallet;
    public int Wallet => _wallet;

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
    }

    public void Add(int amount)
    {
        _wallet += amount;
    }

    private void Minus(int amount)
    {
        _wallet -= amount;
    }

    public void Buy(int price)
    {
        if(_wallet >= price) Minus(price);
    }
    
}
