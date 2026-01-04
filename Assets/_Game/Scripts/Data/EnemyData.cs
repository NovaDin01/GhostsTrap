using UnityEngine;

[CreateAssetMenu(menuName = "Data/EnemyData", fileName = "EnemyInfo")]

public class EnemyData : ScriptableObject
{
    public float speed;
    public int money;
    public EnemyType enemyType;
}