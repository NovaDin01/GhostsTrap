using UnityEngine;

[CreateAssetMenu(menuName = "Data/Movement/RandomMove", fileName = "RandomMoveInfo")]
public class RandomMoveSO : ScriptableObject
{
    [Header("НАСТРОЙКИ ГЕЙМДИЗАЙНА")]
    [Header("Параметры движения")]

    [SerializeField, Tooltip("Базовая скорость движения врага")]
    public float speed;

    [SerializeField, Tooltip("Минимальная дистанция, которую враг проходит перед сменой направления")]
    public float minDistance;

    [SerializeField, Tooltip("Максимальная дистанция, которую враг проходит перед сменой направления")]
    public float maxDistance;
}