using _Game.Scripts.Data;
using UnityEngine;

public enum AttackType
{
    BaseBaton,
    AssaultRifle,
    EnergyBaton
}

public interface IEnemyAttacker
{
    void Init(Player player, Enemy enemy, AttackSettingSO attackSettingSo);
    void Tick();
    bool IsNearTheObject();
}
