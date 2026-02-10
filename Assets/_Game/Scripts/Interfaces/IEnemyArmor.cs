public interface IEnemyArmor
{
    bool HasArmor { get; }
    void ResetArmor();
    bool TryBreakArmor();
}
