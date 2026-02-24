using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReviveOfferWindow : MonoBehaviour
{
    [SerializeField] private float cancelDelay = 3f;

    private GameObject _panel;
    private Button _watchAdButton;
    private Button _cancelButton;
    private TMP_Text _titleText;
    private Coroutine _delayCoroutine;

    public void Show(Action onWatchAd, Action onCancel)
    {
        EnsureUi();
        gameObject.SetActive(true);
        _panel.SetActive(true);

        _watchAdButton.onClick.RemoveAllListeners();
        _watchAdButton.onClick.AddListener(() => onWatchAd?.Invoke());

        _cancelButton.onClick.RemoveAllListeners();
        _cancelButton.onClick.AddListener(() => onCancel?.Invoke());
        _cancelButton.gameObject.SetActive(false);

        if (_delayCoroutine != null)
        {
            StopCoroutine(_delayCoroutine);
        }

        _delayCoroutine = StartCoroutine(ShowCancelButtonDelayed());
    }

    public void Hide()
    {
        if (_delayCoroutine != null)
        {
            StopCoroutine(_delayCoroutine);
            _delayCoroutine = null;
        }

        if (_panel != null)
        {
            _panel.SetActive(false);
        }
    }

    private IEnumerator ShowCancelButtonDelayed()
    {
        yield return new WaitForSecondsRealtime(cancelDelay);
        _cancelButton.gameObject.SetActive(true);
        _delayCoroutine = null;
    }

    private void EnsureUi()
    {
        if (_panel != null) return;

        var canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            var canvasGo = new GameObject("ReviveCanvas");
            canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();
        }

        _panel = new GameObject("ReviveOfferPanel", typeof(RectTransform), typeof(Image));
        _panel.transform.SetParent(canvas.transform, false);

        var panelRect = _panel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        var panelImage = _panel.GetComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.75f);

        _titleText = CreateText("Посмотреть рекламу и продолжить?", new Vector2(0, 120), _panel.transform);
        _watchAdButton = CreateButton("Посмотреть рекламу", new Vector2(0, 20), _panel.transform);
        _cancelButton = CreateButton("Отмена", new Vector2(0, -70), _panel.transform);

        _panel.SetActive(false);
    }

    private TMP_Text CreateText(string message, Vector2 anchoredPos, Transform parent)
    {
        var go = new GameObject("Title", typeof(RectTransform), typeof(TMP_Text));
        go.transform.SetParent(parent, false);

        var rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(900, 120);
        rect.anchoredPosition = anchoredPos;

        var text = go.GetComponent<TMP_Text>();
        text.text = message;
        text.alignment = TextAlignmentOptions.Center;
        text.fontSize = 48;
        text.color = Color.white;

        return text;
    }

    private Button CreateButton(string caption, Vector2 anchoredPos, Transform parent)
    {
        var buttonObj = new GameObject(caption, typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObj.transform.SetParent(parent, false);

        var rect = buttonObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(500, 100);
        rect.anchoredPosition = anchoredPos;

        var image = buttonObj.GetComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0.95f);

        var button = buttonObj.GetComponent<Button>();

        var textObj = new GameObject("Label", typeof(RectTransform), typeof(TMP_Text));
        textObj.transform.SetParent(buttonObj.transform, false);
        var textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        var text = textObj.GetComponent<TMP_Text>();
        text.text = caption;
        text.fontSize = 40;
        text.color = Color.black;
        text.alignment = TextAlignmentOptions.Center;

        return button;
    }
}
