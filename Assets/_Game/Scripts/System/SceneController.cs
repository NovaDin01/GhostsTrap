using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;
    
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

}
