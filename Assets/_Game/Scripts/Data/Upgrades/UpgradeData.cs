using UnityEngine;

[CreateAssetMenu(menuName = "Data/Upgrade", fileName = "UpgradeInfo")]

public class UpgradeData : ScriptableObject
{
    public int cost;
    public int costIncrease;
    public int lvl;
    public int maxLvl;
}