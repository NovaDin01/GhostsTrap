using UnityEngine;

public class MenuUI : MonoBehaviour
{
    public GameObject about;
    
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