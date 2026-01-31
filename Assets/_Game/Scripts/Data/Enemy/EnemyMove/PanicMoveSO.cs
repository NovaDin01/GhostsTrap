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

    [Header("Столкновения")]
    [SerializeField, Tooltip("Слои физических объектов для столкновений")]
    private LayerMask obstacleMask;

    [SerializeField, Tooltip("Радиус проверки препятствий")]
    private float obstacleRadius = 0.2f;
    
    public float Speed => speed;
    public float MinDistance => minDistance;
    public float MaxDistance => maxDistance;
    public LayerMask ObstacleMask => obstacleMask;
    public float ObstacleRadius => obstacleRadius;
}
