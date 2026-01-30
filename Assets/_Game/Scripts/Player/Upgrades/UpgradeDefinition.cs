using UnityEngine;

[System.Serializable]
public class UpgradeDefinition
{
    [SerializeField] private string id;
    [SerializeField] private string displayName;
    [TextArea] [SerializeField] private string description;
    [SerializeField] private Sprite icon;
    [SerializeField] private Upgrade upgrade;

    public string Id => id;
    public string DisplayName => displayName;
    public string Description => description;
    public Sprite Icon => icon;
    public Upgrade Upgrade => upgrade;
}
