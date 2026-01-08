using UnityEngine;

[CreateAssetMenu(menuName = "Data/Movement/ApproachingMove", fileName = "ApproachingMoveInfo")]

public class ApproachingMoveSO : ScriptableObject
{
    [Header("НАСТРОЙКИ ГЕЙМДИЗАЙНА")]
    [Header("Параметры движения")]

    [SerializeField, Tooltip("Базовая скорость движения врага")]
    public float speed;
    
    [SerializeField, Tooltip("Модификатор скорости движения врага без брони")]
    public float multSpeed;

    [SerializeField, Tooltip("Минимальная дистанция, на которую подходит враг к игроку, если вплотную - 0")]
    public float minDistance;
    
    
}