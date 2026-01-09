using _Game.Scripts.Data;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Attack/LongRangeAttack", fileName = "LongRangeAttackInfo")]

public class LongRangeAttackSO : AttackSettingSO
{
    [Header("НАСТРОЙКИ ГЕЙМДИЗАЙНА")]
    [Header("Параметры атаки")]

    [SerializeField, Tooltip("Урон врага")]
    private int damage;

    [SerializeField, Tooltip("Скорость атаки")]
    private float speedFire;

    [SerializeField, Tooltip("Дальность стрельбы")]
    private float rangeFire;
    
    [Header("Настройка пуль")]
    [SerializeField, Tooltip("Кол-во пуль за один раз")]
    private int bulletCount;

    [SerializeField, Tooltip("Частота вылета пуль при атаке")]
    private float bulletDelay;

    [SerializeField, Tooltip("Скорость пули")]
    private float bulletSpeed;
    
    [Header("Не трогать")]
    [SerializeField, Tooltip("Префаб пули")]
    private Bullet bullet;

    public int Damage => damage;
    public float SpeedFire => speedFire;
    public float RangeFire => rangeFire;
    public Bullet Bullet => bullet;
    public float BulletSpeed => bulletSpeed;
    public int BulletCount => bulletCount;
    public float BulletDelay => bulletDelay;
    
}