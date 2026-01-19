using UnityEngine;

public class VisualEffects : MonoBehaviour
{
    public static VisualEffects Instance;

    [Header("Hit Effects")]
    [SerializeField] private ParticleSystem bulletHitEffect;
    [SerializeField] private ParticleSystem playerHitEffect;
    [SerializeField] private ParticleSystem peopleEatEffect;

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
    
    public void PlayPlayerHit(Vector3 transform)
    {
        Instantiate(
            playerHitEffect,
            transform,
            Quaternion.LookRotation(transform)
        );
    }
    
    public void PlayPeopleEat(Vector3 transform)
    {
        Instantiate(
            peopleEatEffect,
            transform,
            Quaternion.LookRotation(transform)
        );
    }
}
