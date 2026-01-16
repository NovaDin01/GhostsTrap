public class TentaclesCount : Upgrade
{
    public override void Get()
    {
        base.Get();
        Player.Instance.UpgradeGridCount();
    }
}