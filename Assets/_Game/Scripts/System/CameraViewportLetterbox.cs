using UnityEngine;

namespace GhostsTrap.System
{
    [RequireComponent(typeof(Camera))]
    public sealed class CameraViewportLetterbox : MonoBehaviour
    {
        [SerializeField] private Vector2Int targetAspect = new Vector2Int(16, 9);

        private Camera _camera;
        private int _lastScreenWidth;
        private int _lastScreenHeight;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            ApplyViewport();
        }

        private void OnEnable()
        {
            ApplyViewport();
        }

        private void OnValidate()
        {
            if (targetAspect.x <= 0 || targetAspect.y <= 0)
            {
                targetAspect = new Vector2Int(16, 9);
            }

            if (_camera == null)
            {
                _camera = GetComponent<Camera>();
            }

            ApplyViewport();
        }

        private void Update()
        {
            if (Screen.width == _lastScreenWidth && Screen.height == _lastScreenHeight)
            {
                return;
            }

            ApplyViewport();
        }

        private void ApplyViewport()
        {
            _lastScreenWidth = Screen.width;
            _lastScreenHeight = Screen.height;

            if (_lastScreenWidth <= 0 || _lastScreenHeight <= 0)
            {
                _camera.rect = new Rect(0f, 0f, 1f, 1f);
                return;
            }

            float target = (float)targetAspect.x / targetAspect.y;
            float window = (float)Screen.width / Screen.height;

            if (!float.IsFinite(target) || !float.IsFinite(window) || target <= 0f || window <= 0f)
            {
                _camera.rect = new Rect(0f, 0f, 1f, 1f);
                return;
            }

            if (window > target)
            {
                float scale = target / window;
                float xOffset = (1f - scale) * 0.5f;
                _camera.rect = new Rect(xOffset, 0f, scale, 1f);
            }
            else
            {
                float scale = window / target;
                float yOffset = (1f - scale) * 0.5f;
                _camera.rect = new Rect(0f, yOffset, 1f, scale);
            }
        }
    }
}
