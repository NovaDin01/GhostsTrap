using UnityEngine;

[CreateAssetMenu(menuName = "Data/Enemy", fileName = "EnemyInfo")]
public class EnemyData : ScriptableObject
{
    [SerializeField] private int hp;
    [SerializeField] private int award;

    [SerializeField, Tooltip("Тип передвижения")]
    private MovementType moveType;

    [SerializeField, Tooltip("Тип атаки")]
    private AttackType attackType;

    public int Hp => hp;
    public int Award => award;
    public MovementType MoveType => moveType;
    public AttackType AttackType => attackType;
}
