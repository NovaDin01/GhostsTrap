using UnityEngine;

[CreateAssetMenu(menuName = "Data/Enemy", fileName = "EnemyInfo")]
public class EnemyData : ScriptableObject
{
    [SerializeField] private int hp;
    [SerializeField] private int award;
    
    [SerializeField, Tooltip("Есть броня?")]
    private bool hasArmor;

    [SerializeField, Tooltip("Тип передвижения")]
    private MovementType moveType;

    [SerializeField, Tooltip("Тип атаки")] 
    private AttackType attackType;

    [SerializeField, Tooltip("Настройки анимаций/эффектов/звука врага")]
    private EnemyFeedbackConfigSO feedbackConfig;

    public int Hp => hp;
    public int Award => award;
    public bool HasArmor => hasArmor;
    public MovementType MoveType => moveType;
    public AttackType AttackType => attackType;
    public EnemyFeedbackConfigSO FeedbackConfig => feedbackConfig;

}
