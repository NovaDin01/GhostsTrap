using UnityEngine;

public class WindowsManager : MonoBehaviour
{
    [SerializeField] private GameObject shop;

    public void SwitchShop()
    {
        SwitchWindow(shop);
    }
    
    private void SwitchWindow(GameObject window)
    {
        window.SetActive(!window.activeSelf);
        PauseManager.Instance.PauseSwitch();
    }

}
