using UnityEngine;

[CreateAssetMenu(menuName = "Data/Enemy", fileName = "EnemyInfo")]
public class EnemyData : ScriptableObject
{
    [SerializeField] public int hp;
    [SerializeField] public int award;
    
    [SerializeField, Tooltip("Есть броня?")]
    public bool hasArmor;
}