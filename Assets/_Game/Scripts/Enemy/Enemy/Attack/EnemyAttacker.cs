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
        ChooseAttackType();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _state = ActionState.Movement;
    }

    public override void Update()
    {
        if (_isCaught) return;
        Tick();
    }

    private void ChooseAttackType()
    {
        switch (enemyData.AttackType)
        {
            case AttackType.BaseBaton:
                _attacker = new BatonAttack();
                _attacker.Init(player, this, setting);
                break;

            case AttackType.AssaultRifle:
                _attacker = new LongRangeAttack();
                _attacker.Init(player, this, setting);
                break;
        }
    }

    public void Tick()
    {
        // сначала выбрать состояние
        bool shouldAttack = _attacker != null && _attacker.IsNearTheObject();
        _state = shouldAttack ? ActionState.Attack : ActionState.Movement;

        // потом выполнить логику состояния
        switch (_state)
        {
            case ActionState.Movement:
                _movement.Tick();
                break;

            case ActionState.Attack:
                _attacker.Tick();
                if(_attacker is LongRangeAttack)
                    _movement.Tick();
                Debug.Log("Attack");
                break;
        }
    }
}