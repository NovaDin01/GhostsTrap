using UnityEngine;

public class UpgradeShopItemData
{
    public string Id { get; }
    public string Name { get; }
    public string Description { get; }
    public Sprite Icon { get; }
    public int Cost { get; }
    public int CostIncrease { get; }
    public int Level { get; }
    public int MaxLevel { get; }
    public bool CanBuy { get; }

    public UpgradeShopItemData(
        string id,
        string name,
        string description,
        Sprite icon,
        int cost,
        int costIncrease,
        int level,
        int maxLevel,
        bool canBuy)
    {
        Id = id;
        Name = name;
        Description = description;
        Icon = icon;
        Cost = cost;
        CostIncrease = costIncrease;
        Level = level;
        MaxLevel = maxLevel;
        CanBuy = canBuy;
    }
}
