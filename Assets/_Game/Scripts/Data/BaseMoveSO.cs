using UnityEngine;

[CreateAssetMenu(menuName = "Data/Movement/BaseMove", fileName = "BaseMoveInfo")]
public class BaseMoveSO : ScriptableObject
{
    [Header("НАСТРОЙКИ ГЕЙМДИЗАЙНА")]
    [Header("Параметры движения")]

    [SerializeField, Tooltip("Базовая скорость движения врага")]
    public float speed;

    [SerializeField, Tooltip("Минимальная дистанция до смены направления")]
    public float minDistance;

    [SerializeField, Tooltip("Максимальная дистанция до смены направления")]
    public float maxDistance;
}