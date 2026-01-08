using UnityEngine;

public enum ActionState
{
    Movement,
    Attack
}

public class EnemyAttacker : Enemy, IAttacker
{
    [SerializeField] private ApproachingMoveSO setting;
    [SerializeField] private AttackData settingAttack;
    [SerializeField] private Transform playerTransform;

    private ActionState _state = ActionState.Movement;

    private float _attackTimer;
    private float _minDistSqr;

    private void OnEnable()
    {
        _attackTimer = 0f;
        _state = ActionState.Movement;
        _minDistSqr = setting.minDistance * setting.minDistance;
    }

    public override void Tick()
    {
        // 1) обновляем таймер
        _attackTimer -= Time.deltaTime;

        // 2) выбираем состояние
        Vector3 diff = playerTransform.position - transform.position;
        float distSqr = diff.sqrMagnitude;

        _state = (distSqr <= _minDistSqr) ? ActionState.Attack : ActionState.Movement;

        // 3) выполняем действие
        switch (_state)
        {
            case ActionState.Movement:
                Move();
                break;

            case ActionState.Attack:
                // атакуем только если кулдаун прошёл
                if (_attackTimer <= 0f)
                {
                    Attack();
                    _attackTimer = settingAttack.speedFire;
                }
                break;
        }
    }

    public override void Move()
    {
        float step = setting.speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, step);
    }

    public void Attack()
    {
        Debug.Log("Удар!");
        // тут: нанести урон/запустить анимацию/событие попадания
    }
}