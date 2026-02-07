// PlayerHealth.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;

    [SerializeField] private List<Image> heartSprites;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;
    [SerializeField] private Sprite nullHeart;

    private bool _subscribed;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        TrySubscribe();
        RedrawFromPlayer();
    }

    private void OnEnable()
    {
        TrySubscribe();
        RedrawFromPlayer();
    }

    private void TrySubscribe()
    {
        if (_subscribed) return;
        if (Player.Instance == null) return;

        Player.Instance.OnHpChanged += HandleHpChanged;
        _subscribed = true;
    }

    private void HandleHpChanged(int current, int max)
    {
        Redraw(current, max);
    }

    private void RedrawFromPlayer()
    {
        if (Player.Instance == null) return;
        Redraw(Player.Instance.CurrentHp, Player.Instance.MaxHp);
    }

    private void Redraw(int currentHp, int maxHp)
    {
        if (heartSprites == null) return;

        for (int i = 0; i < heartSprites.Count; i++)
        {
            if (heartSprites[i] == null) continue;

            if (i >= maxHp)
            {
                heartSprites[i].sprite = nullHeart;
            }
            else
            {
                heartSprites[i].sprite = (i < currentHp) ? fullHeart : emptyHeart;
            }
        }
    }

    private void OnDestroy()
    {
        if (Player.Instance != null && _subscribed)
        {
            Player.Instance.OnHpChanged -= HandleHpChanged;
            _subscribed = false;
        }
    }
}