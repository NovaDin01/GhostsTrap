using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyPresentation : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private Enemy _enemy;

    private static readonly int AnimHit = Animator.StringToHash("Hit");
    private static readonly int AnimCaught = Animator.StringToHash("Caught");
    private static readonly int AnimArmorBreak = Animator.StringToHash("ArmorBreak");

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        _enemy.OnCaught += HandleCaught;
        _enemy.OnArmorBroken += HandleArmorBroken;
    }

    private void OnDisable()
    {
        _enemy.OnCaught -= HandleCaught;
        _enemy.OnArmorBroken -= HandleArmorBroken;
    }

    private void HandleDamaged(int dmg)
    {
        if (animator != null) animator.SetTrigger(AnimHit);
        // SFX:
        // VisualEffects.Instance.PlayEnemyHitSfx(transform.position);
    }

    private void HandleCaught()
    {
        if (animator != null) animator.SetTrigger(AnimCaught);
        // SFX:
        // VisualEffects.Instance.PlayCatchSfx(transform.position);
    }

    private void HandleArmorBroken()
    {
        if (animator != null) animator.SetTrigger(AnimArmorBreak);
        // SFX:
        // VisualEffects.Instance.PlayArmorBreakSfx(transform.position);
    }
}