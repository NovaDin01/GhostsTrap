using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ReviveOfferWindow : MonoBehaviour
{
    [SerializeField] private Button watchAdButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private float cancelDelaySeconds = 3f;

    private Action _watchAdCallback;
    private Action _cancelCallback;
    private Coroutine _cancelDelayRoutine;

    private void Awake()
    {
        if (watchAdButton != null)
        {
            watchAdButton.onClick.AddListener(OnWatchAdClicked);
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(OnCancelClicked);
        }

        gameObject.SetActive(false);
    }

    public void Show(Action watchAdCallback, Action cancelCallback)
    {
        _watchAdCallback = watchAdCallback;
        _cancelCallback = cancelCallback;

        gameObject.SetActive(true);

        if (cancelButton != null)
        {
            cancelButton.gameObject.SetActive(false);
        }

        if (_cancelDelayRoutine != null)
        {
            StopCoroutine(_cancelDelayRoutine);
        }

        _cancelDelayRoutine = StartCoroutine(ShowCancelWithDelay());
    }

    public void Hide()
    {
        if (_cancelDelayRoutine != null)
        {
            StopCoroutine(_cancelDelayRoutine);
            _cancelDelayRoutine = null;
        }

        _watchAdCallback = null;
        _cancelCallback = null;
        gameObject.SetActive(false);
    }

    private IEnumerator ShowCancelWithDelay()
    {
        yield return new WaitForSecondsRealtime(cancelDelaySeconds);

        if (cancelButton != null)
        {
            cancelButton.gameObject.SetActive(true);
        }

        _cancelDelayRoutine = null;
    }

    private void OnWatchAdClicked()
    {
        _watchAdCallback?.Invoke();
    }

    private void OnCancelClicked()
    {
        _cancelCallback?.Invoke();
    }
}
