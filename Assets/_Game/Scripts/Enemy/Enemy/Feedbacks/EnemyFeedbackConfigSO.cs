using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Enemy/Feedbacks", fileName = "EnemyFeedbacks")]
public class EnemyFeedbackConfigSO : ScriptableObject
{
    public EnemyAnimationSettings animation = new();
    public EnemyAudioSettings audio = new();
    public EnemyEffectSettings effects = new();
}

[Serializable]
public class EnemyAnimationSettings
{
    public string moveBool = "IsMoving";
    public string attackTrigger = "Attack";
    public string hitTrigger = "Hit";
    public string deathTrigger = "Death";
    public string caughtTrigger = "Caught";
    public string armorBreakTrigger = "ArmorBreak";
    public string spawnTrigger = "Spawn";
}

[Serializable]
public class EnemyAudioSettings
{
    [Header("One Shots")]
    public AudioClip spawn;
    public AudioClip attack;
    public AudioClip hit;
    public AudioClip death;
    public AudioClip armorBreak;
    public AudioClip caught;

    [Header("Loops")]
    public AudioClip moveLoop;
}

[Serializable]
public class EnemyEffectSettings
{
    public ParticleSystem spawn;
    public ParticleSystem attack;
    public ParticleSystem hit;
    public ParticleSystem death;
    public ParticleSystem armorBreak;
    public ParticleSystem caught;
}
