using UnityEngine;

public class TentaclesRadius : Upgrade
{
    [SerializeField] private float amount;

    public override void Get()
    {
        if (!CanBuy()) return;
        base.Get();
        Player.Instance.UpgradeGridRadius(amount);
    }
}