using _Game.Scripts.Data;
using UnityEngine;

public enum ActionState
{
    Movement,
    Attack
}

public class EnemyAttacker : Enemy
{
    [SerializeField] private AttackSettingSO setting;
    
    private IEnemyAttacker _attacker;

    private ActionState _state = ActionState.Movement;

    public override void Awake()
    {
        base.Awake();
        _state = ActionState.Movement;
        ChooseAttackType();
    }

    public override void Init(Player player)
    {
        base.Init(player);
        ChooseAttackType();
    }

    public override void Update()
    {
        if (_isCaught || IsFrozen) return;
        Tick();
    }

    private void ChooseAttackType()
    {
        switch (enemyData.AttackType)
        {
            case AttackType.BaseBaton:
                _attacker = new BatonAttack();
                _attacker.Init(_player, this, setting);
                break;

            case AttackType.AssaultRifle:
                _attacker = new LongRangeAttack();
                _attacker.Init(_player, this, setting);
                break;
        }
    }

    public void Tick()
    {
        // ñíà÷àëà âûáðàòü ñîñòîÿíèå
        bool shouldAttack = _attacker != null && _attacker.IsNearTheObject();
        _state = shouldAttack ? ActionState.Attack : ActionState.Movement;

        // ïîòîì âûïîëíèòü ëîãèêó ñîñòîÿíèÿ
        switch (_state)
        {
            case ActionState.Movement:
                _movement.Tick();
                break;

            case ActionState.Attack:
                _attacker.Tick();
                if(_attacker is LongRangeAttack)
                    _movement.Tick();
                break;
        }
    }
}