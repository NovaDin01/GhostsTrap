using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeShopRowView : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text desc;
    [SerializeField] private Button buyButton;
    [SerializeField] private TMP_Text buyText;

    private string _id;
    private System.Action<string> _onBuy;

    public void Bind(UpgradeShopItemData data, System.Action<string> onBuy)
    {
        _id = data.Id;
        _onBuy = onBuy;

        title.text = $"{data.Name}  Lvl. {data.Level}/{data.MaxLevel}";
        desc.text = data.Description;

        // Если макс уровень — блокируем
        bool isMax = data.Level >= data.MaxLevel;
        buyButton.interactable = !isMax && data.CanBuy;

        buyText.text = isMax ? "MAX" : $"${data.Cost}";

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => _onBuy?.Invoke(_id));
    }
}