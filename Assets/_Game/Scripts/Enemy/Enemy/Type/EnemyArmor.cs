using UnityEngine;

public class EnemyArmor : MonoBehaviour, IEnemyArmor
{
    [SerializeField] private bool hasArmorOnSpawn = true;
    [SerializeField] private ParticleSystem armorParticleSystem;
    [SerializeField] private GameObject armorEffect;

    private bool _hasArmor;

    public bool HasArmor => _hasArmor;

    private void Awake()
    {
        ResetArmor();
    }

    private void OnEnable()
    {
        ResetArmor();
    }

    public void ResetArmor()
    {
        _hasArmor = hasArmorOnSpawn;

        if (armorParticleSystem != null)
        {
            if (_hasArmor) armorParticleSystem.Play();
            else armorParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        if (armorEffect != null)
        {
            armorEffect.SetActive(_hasArmor);
        }
    }

    public bool TryBreakArmor()
    {
        if (!_hasArmor)
        {
            return false;
        }

        _hasArmor = false;

        if (armorParticleSystem != null)
        {
            armorParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        if (armorEffect != null)
        {
            armorEffect.SetActive(false);
        }

        return true;
    }
}
