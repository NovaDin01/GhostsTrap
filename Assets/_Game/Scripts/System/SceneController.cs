using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;
    public GameObject about;
    
    private const string MENU_SCENE = "Menu";
    private const string GAME_SCENE = "Game";


    private void Awake()
    {
        if(Instance != null && Instance != this) Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        };
    }

    private void OnEnable()
    {
        if (SceneManager.GetActiveScene().name == MENU_SCENE)
        {
            YG2.InterstitialAdvShow();
        }
    }

    private void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(GAME_SCENE);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(MENU_SCENE);
    }
    
    public bool GetScene()
    {
        return (SceneManager.GetActiveScene().name == MENU_SCENE);
    }

    public void OpenAbout()
    {
        about.SetActive(about.activeSelf ? false : true);
    }

}
