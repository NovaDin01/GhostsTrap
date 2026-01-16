using System;
using UnityEngine;
using UnityEngine.UI;

public class BuyButton : MonoBehaviour
{
    [SerializeField] private Upgrade upgrade;
    
    public void Buy()
    {
        upgrade.Get();
    }

}