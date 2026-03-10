using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject SettingsMenu;
    public void ContinueGame()
    {
        SceneManager.LoadScene("MainScene");
        // Load player data
    }

    public void StartGame()
    {
        SceneManager.LoadScene("MainScene");
        // Create new player data
    }

    public void ToggleSettings()
    {
        SettingsMenu.SetActive(!SettingsMenu.activeInHierarchy);
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
