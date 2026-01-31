using UnityEngine;

public class MenuUI : MonoBehaviour
{
    public GameObject about;

    [Header("Audio")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip menuMusic;

    private void Start()
    {
        PlayMenuMusic();
    }

    private void PlayMenuMusic()
    {
        if (menuMusic == null)
        {
            menuMusic = Resources.Load<AudioClip>("Music/menu_music");
        }

        if (menuMusic == null)
        {
            return;
        }

        if (musicSource == null)
        {
            musicSource = GetComponent<AudioSource>();
        }

        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }

        musicSource.loop = true;
        musicSource.spatialBlend = 0f;
        musicSource.clip = menuMusic;

        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }
    
    public void Play()
    {
        SceneController.Instance.LoadGame();
    }

    public void Quit()
    {
        Application.Quit();
    }
    
    public void OpenAbout()
    {
        about.SetActive(about.activeSelf ? false : true);
    }
}
