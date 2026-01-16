using System;
using TMPro;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private Upgrade upgrade;

    [SerializeField] private TMP_Text cost;
    [SerializeField] private TMP_Text lvl;

    private void Update()
    {
        cost.text = $"{upgrade.Cost}";
        lvl.text = $"{upgrade.Lvl} / {upgrade.MaxLvl}";
    }
}
