using _Game.Scripts.Data;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Attack/BatonAttack", fileName = "BatonAttackInfo")]

public class BatonAttackSO : AttackSettingSO
{
    [Header("НАСТРОЙКИ ГЕЙМДИЗАЙНА")]
    [Header("Параметры атаки")]

    [SerializeField, Tooltip("Урон врага")]
    private int damage;

    [SerializeField, Tooltip("Скорость атаки")]
    private float speedFire;

    [SerializeField, Tooltip("Дальность стрельбы")]
    private float rangeFire;

    public int Damage => damage;
    public float SpeedFire => speedFire;
    public float RangeFire => rangeFire;
}