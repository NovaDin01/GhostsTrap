using UnityEngine;

public class VisualEffects : MonoBehaviour
{
    public static VisualEffects Instance;

    [Header("Hit Effects")]
    [SerializeField] private ParticleSystem bulletHitEffect;

    private void Awake()
    {
        if(Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void PlayBulletHit(Vector3 transform)
    {
        Instantiate(
            bulletHitEffect,
            transform,
            Quaternion.LookRotation(transform)
        );
    }
}
