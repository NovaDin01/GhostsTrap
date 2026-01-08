using UnityEngine;

public class EnemyArmor : Enemy, IAttacker
{
    [SerializeField] private ApproachingMoveSO setting;
    [SerializeField] private AttackData settingAttack;
    [SerializeField] private Transform playerTransform;

    private bool _hasArmor;
    private float _distance;
    private float _speed;
    public float Speed => _speed;
    
    private ActionState _state = ActionState.Movement;
    
    private float _minDistSqr;
    private float _attackTimer;
    
    private void OnEnable()
    {
        _attackTimer = 0f;
        _state = ActionState.Movement;
        _distance = setting.minDistance;
        _speed = setting.speed;
        _minDistSqr = _distance * _distance;
        _hasArmor = enemyData.hasArmor;
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

    public override CatchResult TryCatch(Transform catcher)
    {
        
        
        if (_hasArmor)
        {
            BreakArmor();
            return CatchResult.Resisted;
        }
        
        return base.TryCatch(catcher);
        
    }

    protected void BreakArmor()
    {
        _hasArmor = false;

        _distance = 0.5f;
        _minDistSqr = _distance * _distance;

        _speed *= setting.multSpeed;
        
        Debug.Log("Броня уничтожена");
    }



    public override void Move()
    {
        float step = _speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, step);
    }

    
    public void Attack()
    {
        Debug.Log("Удар!");
        // тут: нанести урон/запустить анимацию/событие попадания
    }
}