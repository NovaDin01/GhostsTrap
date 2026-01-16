using UnityEngine;

public class TentaclesSpeed : Upgrade
{
    [SerializeField] private float amount;
    public override void Get()
    {
        base.Get();
        Player.Instance.UpgradeGridSpeed(amount);
    }
}