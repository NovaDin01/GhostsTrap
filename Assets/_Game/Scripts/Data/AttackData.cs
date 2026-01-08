using UnityEngine;

[CreateAssetMenu(menuName = "Data/Attack", fileName = "AttackInfo")]

public class AttackData : ScriptableObject
{
    [Header("НАСТРОЙКИ ГЕЙМДИЗАЙНА")]
    [Header("Параметры атаки")]

    [SerializeField, Tooltip("Урон врага")]
    public int damage;

    [SerializeField, Tooltip("Скорость атаки")]
    public int speedFire;
}