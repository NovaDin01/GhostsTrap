using UnityEngine;

public class VisualEffects : MonoBehaviour
{
    public static VisualEffects Instance;

    [Header("Hit Effects")]
    [SerializeField] private ParticleSystem bulletHitEffect;
    [SerializeField] private ParticleSystem playerHitEffect;
    [SerializeField] private ParticleSystem peopleEatEffect;

    [Header("SFX")]
    [SerializeField] private AudioClip catchClip;
    [SerializeField] private AudioClip throwClip;
    [SerializeField] private AudioClip enemyMeleeAttackClip;

    [SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    // ---------- VFX ----------
    public void PlayBulletHit(Vector3 pos)  => SpawnVfx(bulletHitEffect, pos);
    public void PlayPlayerHit(Vector3 pos)  => SpawnVfx(playerHitEffect, pos);
    public void PlayPeopleEat(Vector3 pos)  => SpawnVfx(peopleEatEffect, pos);

    private void SpawnVfx(ParticleSystem prefab, Vector3 pos)
    {
        if (prefab == null) return;
        Instantiate(prefab, pos, Quaternion.identity);
    }

    // ---------- SFX ----------
    public void PlayCatchSfx(Vector3 pos) => PlayOneShotAtPoint(catchClip, pos);
    public void PlayThrowSfx(Vector3 pos) => PlayOneShotAtPoint(throwClip, pos);
    public void PlayEnemyMeleeAttackSfx(Vector3 pos) => PlayOneShotAtPoint(enemyMeleeAttackClip, pos);

    private void PlayOneShotAtPoint(AudioClip clip, Vector3 pos)
    {
        if (clip == null) return;

        var go = new GameObject("OneShotSFX");
        go.transform.position = pos;

        var src = go.AddComponent<AudioSource>();
        src.spatialBlend = 0f;
        src.volume = sfxVolume;
        src.PlayOneShot(clip);

        Destroy(go, clip.length + 0.05f);
    }
}