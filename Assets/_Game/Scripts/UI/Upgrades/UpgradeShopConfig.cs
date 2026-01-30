using System.Collections.Generic;
using UnityEngine;

public class UpgradeShopConfig : MonoBehaviour
{
    [SerializeField] private List<UpgradeDefinition> upgrades = new List<UpgradeDefinition>();

    public IReadOnlyList<UpgradeDefinition> Upgrades => upgrades;
}
