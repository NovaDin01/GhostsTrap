using System;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyPresentationEvent
{
    Spawn,
    Attack,
    Hit,
    Caught,
    ArmorBroken,
    Death,
    Collected
}

[Serializable]
public class EnemyPresentationEventConfig
{
    public EnemyPresentationEvent EventType;
    public string AnimatorTrigger;
    public AudioClip SfxClip;
    [Range(0f, 1f)] public float SfxVolume = 1f;
    public GameObject VfxPrefab;
    public ParticleSystem ParticlePrefab;
    public bool UseWorldPosition = true;
    public Vector3 PositionOffset;
}

[CreateAssetMenu(menuName = "Data/Enemy/Enemy Presentation Config", fileName = "EnemyPresentationConfig")]
public class EnemyPresentationConfig : ScriptableObject
{
    [SerializeField] private List<EnemyPresentationEventConfig> entries = new();

    public IReadOnlyList<EnemyPresentationEventConfig> Entries => entries;
}
