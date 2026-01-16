public class TentaclesCount : Upgrade
{
    public override void Get()
    {
        if (!CanBuy()) return;
        base.Get();
        Player.Instance.UpgradeGridCount();
    }
}