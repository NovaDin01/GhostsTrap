using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReviveOfferWindow : MonoBehaviour
{
    [SerializeField] private Button watchAdButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text cancelCountdownText;
    [SerializeField] private float cancelDelaySeconds = 3f;

    private Action _onWatchAd;
    private Action _onCancel;
    private Coroutine _cancelRoutine;

    public void Show(Action onWatchAd, Action onCancel)
    {
        _onWatchAd = onWatchAd;
        _onCancel = onCancel;

        if (titleText != null)
            titleText.text = "Посмотреть рекламу и продолжить?";

        if (watchAdButton != null)
        {
            watchAdButton.onClick.RemoveListener(HandleWatchAdClicked);
            watchAdButton.onClick.AddListener(HandleWatchAdClicked);
            watchAdButton.gameObject.SetActive(true);
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveListener(HandleCancelClicked);
            cancelButton.onClick.AddListener(HandleCancelClicked);
            cancelButton.gameObject.SetActive(false);
        }

        if (_cancelRoutine != null)
            StopCoroutine(_cancelRoutine);

        _cancelRoutine = StartCoroutine(ShowCancelAfterDelay());
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (_cancelRoutine != null)
        {
            StopCoroutine(_cancelRoutine);
            _cancelRoutine = null;
        }

        gameObject.SetActive(false);
    }

    private IEnumerator ShowCancelAfterDelay()
    {
        float timer = Mathf.Max(0f, cancelDelaySeconds);

        while (timer > 0f)
        {
            if (cancelCountdownText != null)
                cancelCountdownText.text = $"Отмена через {Mathf.CeilToInt(timer)}";

            timer -= Time.unscaledDeltaTime;
            yield return null;
        }

        if (cancelCountdownText != null)
            cancelCountdownText.text = string.Empty;

        if (cancelButton != null)
            cancelButton.gameObject.SetActive(true);
    }

    private void HandleWatchAdClicked()
    {
        _onWatchAd?.Invoke();
    }

    private void HandleCancelClicked()
    {
        _onCancel?.Invoke();
    }
}
