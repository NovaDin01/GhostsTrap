using System;
using UnityEngine;

public class FlaskStartController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int clicksToBreak = 5;
    [SerializeField] private GameObject monsterPrefab;

    [Header("Optional")]
    [SerializeField] private GameObject breakEffect;
    [SerializeField] private Animator flaskAnimator;

    private int _currentClicks;
    private bool _isBroken;

    private void Awake()
    {
        Time.timeScale = 1;
    }

    private void Update()
    {
        if (_isBroken) return;

        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }

    private void HandleClick()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos);

        if (hit == null) return;
        if (hit.gameObject != gameObject) return;

        _currentClicks++;

        if (flaskAnimator != null)
            flaskAnimator.SetTrigger("Hit");

        if (_currentClicks >= clicksToBreak)
        {
            BreakFlask();
        }
    }

    private void BreakFlask()
    {
        _isBroken = true;

        if (breakEffect != null)
            Instantiate(breakEffect, transform.position, Quaternion.identity);

        Instantiate(monsterPrefab, transform.position, Quaternion.identity);

        StartGame();

        Destroy(gameObject);
    }

    private void StartGame()
    {
        Debug.Log("Игра началась");

        // Примеры:
        Time.timeScale = 1f;
        // GameManager.Instance.StartGame();
        // EnemySpawner.Instance.Enable();
    }
}