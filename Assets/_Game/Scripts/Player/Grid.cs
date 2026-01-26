using System;
using System.Collections.Generic;
using UnityEngine;

public enum GridState
{
    Throwing,
    Stopping,
    Returning
}

public class GridNet : MonoBehaviour
{
    [Header("Rope (Line)")]
    [SerializeField] private LineRenderer rope;
    [SerializeField] private Vector3 ropeStartOffset; // если нужно сместить начало (из "руки")

    private float _speed;
    private float _radius;
    private LayerMask _loots;
    private Vector2 _trap;
    private Vector2 _target;

    private GridState _state;

    private Collider2D[] _enemies = Array.Empty<Collider2D>();
    private readonly List<Collider2D> _caught = new();

    public event Action<GameObject> OnLoot;
    public event Action<GameObject> onBack;

    private void Awake()
    {
        // если не назначил в инспекторе Ч попробуем вз€ть с объекта
        if (rope == null) rope = GetComponent<LineRenderer>();

        if (rope != null)
        {
            rope.positionCount = 2;
            rope.useWorldSpace = true;
            rope.enabled = false; // включаем только когда летим/возвращаемс€
        }
    }

    private void Update()
    {
        State();
        UpdateRope();
    }

    public void Init(float speed, float radius, LayerMask loots, Vector2 trap, Vector2 target)
    {
        _target = target;
        _trap = trap;
        _speed = speed;
        _radius = radius;
        _loots = loots;

        SetDefaultSettings();
    }

    private void SetDefaultSettings()
    {
        transform.position = _trap;
        _state = GridState.Throwing;

        // включаем леску при вылете
        if (rope != null) rope.enabled = true;
        UpdateRope(true);
    }

    private void State()
    {
        switch (_state)
        {
            case GridState.Throwing:
                MoveToTarget();
                break;

            case GridState.Returning:
                MoveToTrap();
                break;

            case GridState.Stopping:
                break;
        }
    }

    private void MoveToTarget()
    {
        GridMove(_target);
        if (ClosingOnTarget(_target))
        {
            _state = GridState.Stopping;
            GetCaught();
        }
    }

    private void MoveToTrap()
    {
        GridMove(_trap);
        if (ClosingOnTarget(_trap))
        {
            _state = GridState.Stopping;
            GetLoot();

            // когда вернулись Ч выключаем леску
            if (rope != null) rope.enabled = false;
        }
    }

    private void GetCaught()
    {
        _caught.Clear();
        _enemies = Physics2D.OverlapCircleAll(transform.position, _radius, _loots);

        foreach (var enemy in _enemies)
        {
            if (enemy == null) continue;

            if (!enemy.TryGetComponent<IObjectAttracted>(out var obj)) continue;

            if (enemy.TryGetComponent<ITakingDamage>(out var takingDamage)) takingDamage.ApplyDamage(1);
            

            var result = obj.TryCatch(transform);
            if (result != CatchResult.Caught)
                continue;

            _caught.Add(enemy);
        }

        _state = GridState.Returning;
    }

    private void GetLoot()
    {
        foreach (var enemy in _caught)
        {
            if (enemy == null) continue;

            enemy.transform.SetParent(null);
            OnLoot?.Invoke(enemy.gameObject);
        }

        onBack?.Invoke(gameObject);

        _enemies = Array.Empty<Collider2D>();
        _caught.Clear();
    }

    private void UpdateRope(bool force = false)
    {
        if (rope == null) return;
        if (!rope.enabled && !force) return;

        Vector3 start = (Vector3)_trap + ropeStartOffset;
        Vector3 end = transform.position;

        rope.SetPosition(0, start);
        rope.SetPosition(1, end);
    }

    private bool ClosingOnTarget(Vector2 point)
    {
        return Vector2.Distance(transform.position, point) < 0.1f;
    }

    private void GridMove(Vector2 target)
    {
        transform.position = Vector2.MoveTowards(transform.position, target, _speed * Time.deltaTime);
    }
}
