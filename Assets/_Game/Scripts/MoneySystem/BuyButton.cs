using UnityEngine;
using UnityEngine.UI;

public class BuyButton : MonoBehaviour
{
    [SerializeField] private Upgrade upgrade;
    [SerializeField] private Button button;

    public void Refresh()
    {
        button.interactable = upgrade.CanBuy();
    }


    public void Buy()
    {
        upgrade.Get();
    }
    
    private void OnEnable()
    {
        MoneySystem.Instance.OnMoneyChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        MoneySystem.Instance.OnMoneyChanged -= Refresh;
    }

}