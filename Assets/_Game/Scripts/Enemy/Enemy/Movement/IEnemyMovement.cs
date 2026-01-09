using _Game.Scripts.Data;

public enum MovementType
{
    PanicType,
    LongRangeType,
    MeleeType
}
public interface IEnemyMovement
{
    void Init(Enemy enemy, Player player, MovementSettingsSO settingsSo);
    void Tick();
}