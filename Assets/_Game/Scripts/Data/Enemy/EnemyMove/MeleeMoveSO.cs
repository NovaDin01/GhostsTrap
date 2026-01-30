using UnityEngine;

namespace _Game.Scripts.Data
{
    
    [CreateAssetMenu(menuName = "Data/Movement/MeleeMove", fileName = "MeleeMoveInfo")]

    public class MeleeMoveSO : MovementSettingsSO
    {
        [Header("НАСТРОЙКИ ГЕЙМДИЗАЙНА")]
        [Header("Параметры движения")]

        [SerializeField, Tooltip("Базовая скорость движения врага")]
        private float speed;
        
        [SerializeField, Tooltip("Модификатор скорости движения врага без брони")]
        private float multSpeed;

        [SerializeField, Tooltip("Дистанция, на которую подходит враг к игроку")]
        private float distance;
        
        public bool FacingRightByDefault = true;
    
        public float Speed => speed;
        public float MultSpeed => multSpeed;
        public float Distance => distance;
    }
}