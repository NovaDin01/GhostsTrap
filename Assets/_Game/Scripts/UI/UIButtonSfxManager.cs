using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIButtonSfxManager : MonoBehaviour
{
    public static UIButtonSfxManager Instance;

    [Header("Audio")]
    [SerializeField] private AudioClip buttonClickClip;
    [SerializeField, Range(0f, 1f)] private float volume = 1f;
    [SerializeField] private AudioSource audioSource;

    private readonly HashSet<Button> _registeredButtons = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureInstance()
    {
        if (Instance != null) return;

        var existing = FindObjectOfType<UIButtonSfxManager>();
        if (existing != null)
        {
            Instance = existing;
            return;
        }

        var go = new GameObject(nameof(UIButtonSfxManager));
        Instance = go.AddComponent<UIButtonSfxManager>();
        DontDestroyOnLoad(go);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void Start()
    {
        RegisterButtonsInScene();
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RegisterButtonsInScene();
    }

    private void RegisterButtonsInScene()
    {
        _registeredButtons.RemoveWhere(button => button == null);

        var buttons = FindObjectsOfType<Button>(true);
        foreach (var button in buttons)
        {
            if (button == null || _registeredButtons.Contains(button)) continue;

            button.onClick.AddListener(PlayClick);
            _registeredButtons.Add(button);
        }
    }

    private void PlayClick()
    {
        var clip = ResolveClip();
        if (clip == null) return;

        EnsureAudioSource();
        audioSource.PlayOneShot(clip, volume);
    }

    private AudioClip ResolveClip()
    {
        if (buttonClickClip != null) return buttonClickClip;

        buttonClickClip = Resources.Load<AudioClip>("UI/button_click");
        if (buttonClickClip != null) return buttonClickClip;

        if (VisualEffects.Instance != null)
        {
            buttonClickClip = VisualEffects.Instance.ButtonClickClip;
        }

        return buttonClickClip;
    }

    private void EnsureAudioSource()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.spatialBlend = 0f;
    }
}
