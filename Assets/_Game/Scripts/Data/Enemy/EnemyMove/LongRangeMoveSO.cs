using _Game.Scripts.Data;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Movement/LongRangeMove", fileName = "LongRangeMoveInfo")]

public class LongRangeMoveSO : MovementSettingsSO
{
    [Header("НАСТРОЙКИ ГЕЙМДИЗАЙНА")]
    [Header("Параметры движения")]

    [SerializeField, Tooltip("Базовая скорость движения врага")]
    private float speed;
    
    [SerializeField, Tooltip("Угловая скорость движения врага")]
    private float angularSpeed;
    
    [SerializeField, Tooltip("Модификатор скорости движения врага без брони")]
    private float multSpeed;

    [SerializeField, Tooltip("Дистанция, на которую подходит враг к игроку")]
    private float distance;
    
    public float Speed => speed;
    public float AngularSpeed => angularSpeed;
    public float MultSpeed => multSpeed;
    public float Distance => distance;



}