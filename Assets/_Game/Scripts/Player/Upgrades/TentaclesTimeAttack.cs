using UnityEngine;

public class TentaclesTimeAttack : Upgrade
{
    [SerializeField] private float amount;
    public override void Get()
    {
        if (!CanBuy()) return;
        base.Get();
        Player.Instance.UpgradeGridTime2Attack(amount);
    }
}