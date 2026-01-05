using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
// Скрипт управления ловушкой: бросок, захват, перенос, побег и доставка
public class TrapController2 : MonoBehaviour
{
    [Header("Состояния ловушки")]
    private bool _isFlying = false; 
    private bool _isCatching = false;
    private bool _isReturning = false;

    [Header("Ссылки")]
    [SerializeField] private GameObject _trapCirclePrefab;
    [SerializeField] private CursorController _cursorController;
    [SerializeField] private Transform _trapBoxPosition;
    [SerializeField] private GhostBehaviour _ghost;
    [SerializeField] private EnemyPool _enemyPool;

    [Header("Характеристики ловушки")]
    [SerializeField] private float _trapMoveSpeed = 2f;
    [SerializeField] private float _returnToBoxSpeed = 2f; 
    [SerializeField] private float _catchRadius = 1f; // радиус захвата 
    [SerializeField] private Sprite _openSprite;
    [SerializeField] private Sprite _closedSprite;

    [Header("Дополнения")]
    private GameObject _actualTrapCircle;
    private Vector3 _targetPosition;
    private LineRenderer _lineRenderer;
    private void Start()
    {
        _cursorController.SetTrapCursor();
        _lineRenderer = GetComponent<LineRenderer>();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !_isFlying && !_isReturning) SpawnTrapCircle();
        if (_isFlying) MoveTrapCircle();
        if (_isReturning) ReturnMoveTrapCircle();
    }
    private void SpawnTrapCircle() //Метод создания ловушки, создаем таргет позицию мыши, чтобы при клике ловушка не летела за курсором
    {
        _targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _targetPosition.z = 0;
        _actualTrapCircle = Instantiate(_trapCirclePrefab, _trapBoxPosition.position, Quaternion.identity);
        _actualTrapCircle.GetComponent<SpriteRenderer>().sprite = _openSprite;
        _isFlying = true;
        _cursorController.SetDefaultCursor();
    }
    private void MoveTrapCircle() //Метод передвижения ловушки к месту, куда щелкнули мышью
    {
        _actualTrapCircle.transform.position = Vector3.MoveTowards(_actualTrapCircle.transform.position, _targetPosition, _trapMoveSpeed * Time.deltaTime);
        float distance = Vector3.Distance(_actualTrapCircle.transform.position, _targetPosition);
        if (distance < 0.1f)
        {
            _isFlying = false;
            _cursorController.SetTrapCursor();
            _ghost = FindFirstObjectByType<GhostBehaviour>(); // !!!
            float distanceToGhost = Vector2.Distance(_actualTrapCircle.transform.position, _ghost.transform.position);
            if (distanceToGhost <= _catchRadius)
            {
                CatchGhost();
            }
        }
    }
    private void CatchGhost() //Метод захвата призрака
    {
        _ghost.transform.SetParent(_actualTrapCircle.transform);
        _ghost.transform.localPosition = Vector3.zero;
        _isReturning = true;
   }
   private void ReturnMoveTrapCircle() //Метода перетягивания призрака к ловушке
    {
        _actualTrapCircle.transform.position = Vector3.MoveTowards(_actualTrapCircle.transform.position, _trapBoxPosition.position, _returnToBoxSpeed * Time.deltaTime);
        float distance = Vector3.Distance(_actualTrapCircle.transform.position, _trapBoxPosition.position);
        if (distance < 0.1f)
        {
            _isReturning = false;
            //_enemyPool.ReturnObject(_actualTrapCircle.transform.GetChild(0).gameObject); // !!!
            _actualTrapCircle.transform.DetachChildren();
            _actualTrapCircle = null;
        }
    }
    private void SpawnLineTrap ()
    {
        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(0, _trapBoxPosition.position);
        _lineRenderer.SetPosition(1, _targetPosition);
    }
}
