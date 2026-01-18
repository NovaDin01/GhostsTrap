using UnityEngine;

public class MenuUI : MonoBehaviour
{
    public void Play()
    {
        SceneController.Instance.LoadGame();
    }

    public void Quit()
    {
        Application.Quit();
    }
}