using _Game.Scripts.Data;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Movement/PanicMove", fileName = "PanicMoveInfo")]
public class PanicMoveSO : MovementSettingsSO
{
    [Header("НАСТРОЙКИ ГЕЙМДИЗАЙНА")]
    [Header("Параметры движения")]

    [SerializeField, Tooltip("Базовая скорость движения врага")]
    private float speed;

    [SerializeField, Tooltip("Минимальная дистанция, которую враг проходит перед сменой направления")]
    private float minDistance;

    [SerializeField, Tooltip("Максимальная дистанция, которую враг проходит перед сменой направления")]
    private float maxDistance;
    
    public float Speed => speed;
    public float MinDistance => minDistance;
    public float MaxDistance => maxDistance;
}