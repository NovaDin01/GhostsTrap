using UnityEngine;

public class EnemyFeedbacks : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private EnemyFeedbackConfigSO config;

    [Header("Bindings")]
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource oneShotSource;
    [SerializeField] private AudioSource loopSource;
    [SerializeField] private Transform effectAnchor;

    [Header("Movement Detection")]
    [SerializeField] private float moveThreshold = 0.01f;

    private Vector3 _lastPosition;
    private bool _isMoving;
    private bool _isDead;
    private bool _movementLocked;

    public EnemyFeedbackConfigSO Config => config;

    public void ApplyConfig(EnemyFeedbackConfigSO newConfig)
    {
        if (newConfig != null)
        {
            config = newConfig;
        }
    }

    private void Awake()
    {
        if (effectAnchor == null)
        {
            effectAnchor = transform;
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (oneShotSource == null)
        {
            oneShotSource = GetComponentInChildren<AudioSource>();
        }

        if (loopSource == null)
        {
            loopSource = oneShotSource;
        }
    }

    private void OnEnable()
    {
        _lastPosition = transform.position;
        _isMoving = false;
        _isDead = false;
        _movementLocked = false;
        PlaySpawn();
    }

    private void Update()
    {
        if (_isDead || _movementLocked)
        {
            return;
        }

        UpdateMovement();
    }

    private void UpdateMovement()
    {
        Vector3 currentPosition = transform.position;
        float movedSqr = (currentPosition - _lastPosition).sqrMagnitude;
        bool movingNow = movedSqr > moveThreshold * moveThreshold;

        if (movingNow != _isMoving)
        {
            _isMoving = movingNow;
            SetMoveState(_isMoving);
        }

        _lastPosition = currentPosition;
    }

    private void SetMoveState(bool isMoving)
    {
        if (config != null && animator != null && !string.IsNullOrWhiteSpace(config.animation.moveBool))
        {
            animator.SetBool(config.animation.moveBool, isMoving);
        }

        if (config == null || loopSource == null)
        {
            return;
        }

        if (config.audio.moveLoop == null)
        {
            if (loopSource.isPlaying && loopSource.loop)
            {
                loopSource.Stop();
            }

            return;
        }

        loopSource.loop = true;
        if (isMoving)
        {
            if (loopSource.clip != config.audio.moveLoop)
            {
                loopSource.clip = config.audio.moveLoop;
            }

            if (!loopSource.isPlaying)
            {
                loopSource.Play();
            }
        }
        else
        {
            if (loopSource.isPlaying)
            {
                loopSource.Stop();
            }
        }
    }

    public void PlaySpawn()
    {
        if (config == null)
        {
            return;
        }

        TriggerAnimation(config.animation.spawnTrigger);
        PlayOneShot(config.audio.spawn);
        SpawnEffect(config.effects.spawn);
    }

    public void PlayAttack()
    {
        if (config == null)
        {
            return;
        }

        TriggerAnimation(config.animation.attackTrigger);
        PlayOneShot(config.audio.attack);
        SpawnEffect(config.effects.attack);
    }

    public void PlayHit()
    {
        if (config == null)
        {
            return;
        }

        TriggerAnimation(config.animation.hitTrigger);
        PlayOneShot(config.audio.hit);
        SpawnEffect(config.effects.hit);
    }

    public void PlayDeath()
    {
        if (config == null)
        {
            _isDead = true;
            return;
        }

        _isDead = true;
        SetMoveState(false);
        TriggerAnimation(config.animation.deathTrigger);
        PlayOneShot(config.audio.death);
        SpawnEffect(config.effects.death);
    }

    public void PlayArmorBreak()
    {
        if (config == null)
        {
            return;
        }

        TriggerAnimation(config.animation.armorBreakTrigger);
        PlayOneShot(config.audio.armorBreak);
        SpawnEffect(config.effects.armorBreak);
    }

    public void PlayCaught()
    {
        if (config == null)
        {
            return;
        }

        _movementLocked = true;
        SetMoveState(false);
        TriggerAnimation(config.animation.caughtTrigger);
        PlayOneShot(config.audio.caught);
        SpawnEffect(config.effects.caught);
    }

    private void TriggerAnimation(string trigger)
    {
        if (animator == null || string.IsNullOrWhiteSpace(trigger))
        {
            return;
        }

        animator.SetTrigger(trigger);
    }

    private void PlayOneShot(AudioClip clip)
    {
        if (oneShotSource == null || clip == null)
        {
            return;
        }

        oneShotSource.PlayOneShot(clip);
    }

    private void SpawnEffect(ParticleSystem effect)
    {
        if (effect == null || effectAnchor == null)
        {
            return;
        }

        Instantiate(effect, effectAnchor.position, effectAnchor.rotation);
    }
}
