using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyPresentation : MonoBehaviour
{
    [SerializeField] private EnemyPresentationConfig config;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Transform vfxRoot;

    private Enemy _enemy;

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (audioSource == null) audioSource = GetComponentInChildren<AudioSource>();
        if (vfxRoot == null) vfxRoot = transform;
    }

    private void OnEnable()
    {
        _enemy.OnAttack += HandleAttack;
        _enemy.OnDamaged += HandleDamaged;
        _enemy.OnCaught += HandleCaught;
        _enemy.OnArmorBroken += HandleArmorBroken;
        _enemy.OnDeath += HandleDeath;
        _enemy.OnCollectedEnemy += HandleCollected;

        Play(EnemyPresentationEvent.Spawn);
    }

    private void OnDisable()
    {
        _enemy.OnAttack -= HandleAttack;
        _enemy.OnDamaged -= HandleDamaged;
        _enemy.OnCaught -= HandleCaught;
        _enemy.OnArmorBroken -= HandleArmorBroken;
        _enemy.OnDeath -= HandleDeath;
        _enemy.OnCollectedEnemy -= HandleCollected;
    }

    private void HandleAttack()
    {
        Play(EnemyPresentationEvent.Attack);
    }

    private void HandleDamaged(int dmg)
    {
        Play(EnemyPresentationEvent.Hit);
    }

    private void HandleCaught()
    {
        Play(EnemyPresentationEvent.Caught);
    }

    private void HandleArmorBroken()
    {
        Play(EnemyPresentationEvent.ArmorBroken);
    }

    private void HandleDeath()
    {
        Play(EnemyPresentationEvent.Death);
    }

    private void HandleCollected(Enemy enemy)
    {
        Play(EnemyPresentationEvent.Collected);
    }

    private void Play(EnemyPresentationEvent eventType)
    {
        if (config == null) return;

        foreach (var entry in config.Entries)
        {
            if (entry.EventType != eventType) continue;

            if (animator != null && !string.IsNullOrWhiteSpace(entry.AnimatorTrigger))
            {
                animator.SetTrigger(entry.AnimatorTrigger);
            }

            if (entry.SfxClip != null)
            {
                PlaySfx(entry.SfxClip, entry.SfxVolume);
            }

            if (entry.VfxPrefab != null)
            {
                SpawnPrefab(entry.VfxPrefab, entry);
            }

            if (entry.ParticlePrefab != null)
            {
                SpawnParticle(entry.ParticlePrefab, entry);
            }
        }
    }

    private void PlaySfx(AudioClip clip, float volume)
    {
        if (audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
            return;
        }

        var go = new GameObject("EnemySfx");
        go.transform.position = transform.position;

        var src = go.AddComponent<AudioSource>();
        src.spatialBlend = 0f;
        src.volume = volume;
        src.PlayOneShot(clip);
        Destroy(go, clip.length + 0.05f);
    }

    private void SpawnPrefab(GameObject prefab, EnemyPresentationEventConfig entry)
    {
        if (entry.UseWorldPosition)
        {
            Instantiate(prefab, transform.position + entry.PositionOffset, Quaternion.identity);
            return;
        }

        var instance = Instantiate(prefab, vfxRoot);
        instance.transform.localPosition = entry.PositionOffset;
    }

    private void SpawnParticle(ParticleSystem prefab, EnemyPresentationEventConfig entry)
    {
        if (entry.UseWorldPosition)
        {
            Instantiate(prefab, transform.position + entry.PositionOffset, Quaternion.identity);
            return;
        }

        var instance = Instantiate(prefab, vfxRoot);
        instance.transform.localPosition = entry.PositionOffset;
    }
}
